using GitExtensions.BundleBackuper.Services;
using GitExtensions.BundleBackuper.UI;
using GitUI;
using GitUI.CommandsDialogs;
using GitUIPluginInterfaces;
using ResourceManager;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

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

        public override void Register(IGitUICommands commands)
        {
            base.Register(commands);
            Configuration = new PluginSettings(Settings);

            FormBrowse form = (FormBrowse)commands.BrowseRepo;
            MenuStripEx mainMenu = form.Controls.OfType<MenuStripEx>().FirstOrDefault();
            if (mainMenu != null)
            {
                if (!mainMenu.Items.OfType<BundleToolStripMenuItem>().Any())
                    mainMenu.Items.Add(new BundleToolStripMenuItem(new FileSystemBundleProvider(Configuration), commands));
            }
        }

        public override void Unregister(IGitUICommands gitUiCommands)
        {
            base.Unregister(gitUiCommands);
        }
    }
}
