using GitCommands;
using GitUI;
using GitUI.CommandsDialogs;
using GitUIPluginInterfaces;
using Neptuo;
using Neptuo.Activators;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GitExtensions.BundleBackuper.Services
{
    public class GitUiCommandsBundleService : IGitBundleMapper, IGitBundleMapperNotification, IGitBundleFactory
    {
        private readonly IFactory<GitUICommands> commandsFactory;
        private readonly IBundleNameProvider nameProvider;

        public event Action<Bundle> Added;
        public event Action<Bundle> Removed;

        public GitUiCommandsBundleService(IFactory<GitUICommands> commandsFactory, IBundleNameProvider nameProvider)
        {
            Ensure.NotNull(commandsFactory, "commandsFactory");
            Ensure.NotNull(nameProvider, "nameProvider");
            this.commandsFactory = commandsFactory;
            this.nameProvider = nameProvider;
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

        public async Task<Bundle> CreateAsync()
        {
            GitUICommands commands = commandsFactory.Create();
            string commitId = await FindLastPushedCommitIdAsync(commands);
            if (commitId != null)
            {
                Bundle bundle = nameProvider.Get();
                commands.StartGitCommandProcessDialog((FormBrowse)commands.BrowseRepo, $"bundle create {bundle.FilePath} {commitId}..{commands.GitModule.GetSelectedBranch()}");
                return bundle;
            }

            return null;
        }

        private Task<string> FindLastPushedCommitIdAsync(IGitUICommands commands)
        {
            return Task.Factory.StartNew(() =>
            {
                string commitId = FindCommitIdIfPushed(commands, 0);
                if (!String.IsNullOrWhiteSpace(commitId))
                    return null;

                int step = 20;
                for (int i = step; i < 2000; i += step)
                {
                    if (IsCommitPushed(commands, i))
                    {
                        int headOffset = BinarySearch(i - step, i, offset =>
                        {
                            if (IsCommitPushed(commands, offset))
                            {
                                if (IsCommitPushed(commands, offset - 1))
                                    return -1;
                                else
                                    return 0;
                            }
                            else
                            {
                                return 1;
                            }
                        });
                        commitId = FindCommitId(commands, headOffset);
                        return commitId;
                    }
                }

                return null;
            });
        }

        private string FindCommitIdIfPushed(IGitUICommands commands, int headOffset)
        {
            if (IsCommitPushed(commands, headOffset))
            {
                string commitId = commands.GitModule.RunGitCmd($"rev-parse HEAD~{headOffset}").Trim();
                if (!String.IsNullOrWhiteSpace(commitId))
                    return commitId;
            }

            return null;
        }

        private string FindCommitId(IGitUICommands commands, int headOffset)
        {
            string commitId = commands.GitModule.RunGitCmd($"rev-parse HEAD~{headOffset}").Trim();
            if (!String.IsNullOrWhiteSpace(commitId) && !commitId.Contains(" "))
                return commitId;

            return null;
        }

        private bool IsCommitPushed(IGitUICommands commands, int headOffset)
        {
            string commitId = FindCommitId(commands, headOffset);
            if (string.IsNullOrEmpty(commitId))
                return true;

            string branches = commands.GitModule.RunGitCmd($"branch -r --contains {commitId}");
            return !String.IsNullOrWhiteSpace(branches);
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
    }
}
