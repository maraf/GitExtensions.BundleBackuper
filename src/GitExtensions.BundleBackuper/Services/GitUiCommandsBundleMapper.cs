using GitUIPluginInterfaces;
using Neptuo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GitExtensions.BundleBackuper.Services
{
    public class GitUiCommandsBundleMapper : IGitBundleMapper
    {
        private readonly IGitUICommands commands;

        public GitUiCommandsBundleMapper(IGitUICommands commands)
        {
            Ensure.NotNull(commands, "commands");
            this.commands = commands;
        }

        public bool Has(Bundle bundle)
            => commands.GitModule.GetRemotes().Contains(bundle.Name);

        public void Add(Bundle bundle)
        {
            commands.GitModule.AddRemote(bundle.Name, bundle.FilePath);
            commands.StartGitCommandProcessDialog("fetch --all");
            commands.RepoChangedNotifier.Notify();
        }

        public void Remove(Bundle bundle)
        {
            if (Has(bundle))
            {
                commands.GitModule.RemoveRemote(bundle.Name);
                commands.StartGitCommandProcessDialog("fetch --all");
                commands.RepoChangedNotifier.Notify();
            }
        }
    }
}
