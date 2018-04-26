using GitUIPluginInterfaces;
using ResourceManager;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GitExtensions.BundleBackuper
{
    public class Plugin : GitPluginBase, IGitPluginForRepository
    {
        internal PluginSettings Configuration { get; private set; }

        public Plugin()
        {
            Name = "Bundle Backuper";
            Description = "Branch backuping extension for GitExtensions";
        }

        public override bool Execute(GitUIBaseEventArgs gitUiCommands)
            => true;

        public override IEnumerable<ISetting> GetSettings()
            => Configuration;

        public override void Register(IGitUICommands gitUiCommands)
        {
            base.Register(gitUiCommands);
            Configuration = new PluginSettings(Settings);
        }

        public override void Unregister(IGitUICommands gitUiCommands)
        {
            base.Unregister(gitUiCommands);
        }
    }
}
