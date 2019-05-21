using GitUI;
using GitUI.CommandsDialogs;
using GitUIPluginInterfaces;
using Neptuo;
using Neptuo.Activators;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GitExtensions.BundleBackuper.Services
{
    public partial class GitUiCommandsBundleService : IGitBundleMapper, IGitBundleMapperNotification, IGitBundleFactory, IGitBundleFactoryNotification
    {
        private readonly IFactory<GitUICommands> commandsFactory;
        private readonly IBundleNameProvider nameProvider;
        private readonly PluginSettings settings;

        public event Action<Bundle> Added;
        public event Action<Bundle> Removed;
        public event Action<Bundle, CancelEventArgs> Creating;
        public event Action<Bundle> Created;

        internal GitUiCommandsBundleService(IFactory<GitUICommands> commandsFactory, IBundleNameProvider nameProvider, PluginSettings settings)
        {
            Ensure.NotNull(commandsFactory, "commandsFactory");
            Ensure.NotNull(nameProvider, "nameProvider");
            Ensure.NotNull(settings, "settings");
            this.commandsFactory = commandsFactory;
            this.nameProvider = nameProvider;
            this.settings = settings;
        }

        public bool Has(Bundle bundle)
            => commandsFactory.Create().GitModule.GetRemoteNames().Contains(bundle.Name);

        public void Add(Bundle bundle)
        {
            if (!Has(bundle))
            {
                IGitUICommands commands = commandsFactory.Create();
                commands.GitModule.AddRemote(bundle.Name, bundle.FilePath);
                Added?.Invoke(bundle);
            }
        }

        public void Remove(Bundle bundle)
        {
            if (Has(bundle))
            {
                IGitUICommands commands = commandsFactory.Create();
                commands.GitModule.RemoveRemote(bundle.Name);
                Removed?.Invoke(bundle);
            }
        }

        public Task<Bundle> CreateAsync()
            => CreateAsync("HEAD");

        public async Task<Bundle> CreateAsync(string referenceName)
        {
            Ensure.NotNullOrEmpty(referenceName, "referenceName");

            GitUICommands commands = commandsFactory.Create();
            Tuple<FindCommitResult, string> result = await FindLastPushedCommitIdAsync(commands, referenceName);
            if (result.Item1 != FindCommitResult.NotFound)
            {
                if (referenceName.Equals("head", StringComparison.InvariantCultureIgnoreCase))
                    referenceName = commands.GitModule.GetSelectedBranch();

                Bundle bundle = nameProvider.Get(referenceName);

                if (Creating != null)
                {
                    var args = new CancelEventArgs();
                    Creating(bundle, args);

                    if (args.Cancel)
                        return null;
                }

                string arguments = null;
                if (result.Item1 == FindCommitResult.BaseFound)
                    arguments = $"bundle create {bundle.FilePath} {result.Item2}..{referenceName}";
                else if (result.Item1 == FindCommitResult.WithoutBase)
                    arguments = $"bundle create {bundle.FilePath} {referenceName}";
                else
                    Ensure.Exception.NotSupported(result.Item1);

                commands.StartGitCommandProcessDialog((FormBrowse)commands.BrowseRepo, arguments);

                Created?.Invoke(bundle);
                return bundle;
            }

            return null;
        }

        private Task<Tuple<FindCommitResult, string>> FindLastPushedCommitIdAsync(IGitUICommands commands, string head)
        {
            return Task.Factory.StartNew(() =>
            {
                string commitId = FindCommitIdIfPushed(commands, head, 0);
                if (!String.IsNullOrWhiteSpace(commitId))
                    return new Tuple<FindCommitResult, string>(FindCommitResult.NotFound, (string)null);

                int step = 20;
                for (int i = step; i < 2000; i += step)
                {
                    if (IsCommitPushed(commands, head, i))
                    {
                        int headOffset = BinarySearch(i - step, i, offset =>
                        {
                            if (IsCommitPushed(commands, head, offset))
                            {
                                if (IsCommitPushed(commands, head, offset - 1))
                                    return -1;
                                else
                                    return 0;
                            }
                            else
                            {
                                return 1;
                            }
                        });

                        if (headOffset == 0)
                            return new Tuple<FindCommitResult, string>(FindCommitResult.NotFound, null);

                        commitId = FindCommitId(commands, head, headOffset);
                        if (commitId == null)
                            return new Tuple<FindCommitResult, string>(FindCommitResult.WithoutBase, null);

                        return new Tuple<FindCommitResult, string>(FindCommitResult.BaseFound, commitId);
                    }
                }

                return new Tuple<FindCommitResult, string>(FindCommitResult.NotFound, null);
            });
        }

        private string FindCommitIdIfPushed(IGitUICommands commands, string head, int headOffset)
        {
            if (IsCommitPushed(commands, head, headOffset))
                return FindCommitId(commands, head, headOffset);

            return null;
        }

        private string FindCommitId(IGitUICommands commands, string head, int headOffset)
        {
            string commitId = RunGitCommand(commands, $"rev-parse {head}~{headOffset}").Trim();
            if (!String.IsNullOrWhiteSpace(commitId) && !commitId.Contains(" "))
                return commitId;

            return null;
        }

        private bool IsCommitPushed(IGitUICommands commands, string head, int headOffset)
        {
            string commitId = FindCommitId(commands, head, headOffset);
            if (String.IsNullOrEmpty(commitId))
                return true;

            

            string branches = RunGitCommand(commands, $"branch -r --contains {commitId}");
            if (String.IsNullOrWhiteSpace(branches))
                return false;

            if (settings.RemoteNamesToCheck.Count > 0)
            {
                string[] list = branches.Split(new[] { Environment.NewLine, "\n" }, StringSplitOptions.RemoveEmptyEntries);
                foreach (string branch in list)
                {
                    string b = branch.Trim();
                    int indexOfSlash = b.IndexOf('/');
                    if (indexOfSlash > 0)
                    {
                        string remoteName = b.Substring(0, indexOfSlash);
                        if (settings.RemoteNamesToCheck.Contains(remoteName))
                            return true;
                    }
                }

                return false;
            }

            return true;
        }

        private int BinarySearch(int start, int end, Func<int, int> predicate)
        {
            Ensure.PositiveOrZero(start, "start");
            Ensure.Positive(end, "end");
            if (start >= end)
                throw Ensure.Exception.Argument("end", "End must be greater than start.");

            int diff = end - start;
            int median = start + (diff / 2);

            switch (predicate(median))
            {
                case -1:
                    return BinarySearch(start, median, predicate);
                case 1:
                    return BinarySearch(median, end, predicate);
                case 0:
                    return median;
                default:
                    throw Ensure.Exception.NotSupported("BinarySearch predicate must return -1, 0 or 1.");
            }
        }

        private string RunGitCommand(IGitUICommands commands, string arguments)
        {
            using (IProcess process = commands.GitModule.GitCommandRunner.RunDetached(arguments, redirectOutput: true))
            {
                process.WaitForExit();

                string error = process.StandardError.ReadToEnd();
                if (!String.IsNullOrEmpty(error))
                    return error;

                return process.StandardOutput.ReadToEnd();
            }
        }
    }
}
