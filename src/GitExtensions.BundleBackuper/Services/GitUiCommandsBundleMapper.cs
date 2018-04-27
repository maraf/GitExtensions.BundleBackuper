using GitUIPluginInterfaces;
using Neptuo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GitExtensions.BundleBackuper.Services
{
    public class GitUiCommandsBundleMapper : IGitBundleMapper, IGitBundleMapperNotification
    {
        private readonly IGitUICommands commands;

        public event Action<Bundle> Added;
        public event Action<Bundle> Removed;

        public GitUiCommandsBundleMapper(IGitUICommands commands)
        {
            Ensure.NotNull(commands, "commands");
            this.commands = commands;
        }

        public bool Has(Bundle bundle)
            => commands.GitModule.GetRemotes().Contains(bundle.Name);

        public void Add(Bundle bundle)
        {
            if (!Has(bundle))
            {
                commands.GitModule.AddRemote(bundle.Name, bundle.FilePath);
                Added?.Invoke(bundle);
            }
        }

        public void Remove(Bundle bundle)
        {
            if (Has(bundle))
            {
                commands.GitModule.RemoveRemote(bundle.Name);
                Removed?.Invoke(bundle);
            }
        }
    }
}
