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
    public class GitUiCommandsBundleMapper : IGitBundleMapper, IGitBundleMapperNotification
    {
        private readonly IFactory<IGitUICommands> commandsFactory;

        public event Action<Bundle> Added;
        public event Action<Bundle> Removed;

        public GitUiCommandsBundleMapper(IFactory<IGitUICommands> commandsFactory)
        {
            Ensure.NotNull(commandsFactory, "commandsFactory");
            this.commandsFactory = commandsFactory;
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
    }
}
