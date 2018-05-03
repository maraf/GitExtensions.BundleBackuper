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
        private readonly IFactory<IGitUICommands> commandsFactory;
        private readonly IBundleNameProvider nameProvider;

        public event Action<Bundle> Added;
        public event Action<Bundle> Removed;

        public GitUiCommandsBundleService(IFactory<IGitUICommands> commandsFactory, IBundleNameProvider nameProvider)
        {
            Ensure.NotNull(commandsFactory, "commandsFactory");
            Ensure.NotNull(nameProvider, "nameProvider");
            this.commandsFactory = commandsFactory;
            this.nameProvider = nameProvider;
        }

        public bool Has(Bundle bundle)
            => commandsFactory.Create().GitModule.GetRemotes().Contains(bundle.Name);

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
            IGitUICommands commands = commandsFactory.Create();
            string commitId = await FindLastPushedCommitIdAsync(commands);
            if (commitId != null)
            {
                Bundle bundle = nameProvider.Get();
                commands.StartGitCommandProcessDialog($"bundle create {bundle.FilePath} {commitId}..HEAD");
                return bundle;
            }

            return null;
        }

        private Task<string> FindLastPushedCommitIdAsync(IGitUICommands commands)
        {
            return Task.Factory.StartNew(() =>
            {
                int i = 0;
                while (true)
                {
                    string commitId = FindCommitIdIfPushed(commands, i);
                    if (!String.IsNullOrWhiteSpace(commitId))
                    {
                        if (i > 0)
                            return commitId;
                        else
                            break;
                    }
                    else if (i > 1000) // TODO: Config or something.
                    {
                        break;
                    }

                    i++;
                }

                return null;
            });
        }

        private string FindCommitIdIfPushed(IGitUICommands commands, int headOffset)
        {
            string branches = commands.GitCommand($"branch -r --contains HEAD~{headOffset}");

            if (!String.IsNullOrWhiteSpace(branches))
            {
                string commitId = commands.GitCommand($"rev-parse HEAD~{headOffset}").Trim();
                if (!String.IsNullOrWhiteSpace(commitId))
                    return commitId;
            }

            return null;
        }
    }
}
