using GitExtensions.BundleBackuper.Services;
using GitExtensions.BundleBackuper.UI;
using GitUI;
using GitUI.CommandsDialogs;
using GitUIPluginInterfaces;
using Neptuo;
using Neptuo.Activators;
using ResourceManager;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace GitExtensions.BundleBackuper
{
    /// <summary>
    /// GitExtensions plugin for backuping using bundles.
    /// </summary>
    public class Plugin : GitPluginBase, IGitPluginForRepository, IFactory<IGitUICommands>
    {
        private readonly List<IDisposable> disposables = new List<IDisposable>();
        private IGitUICommands commands;

        internal PluginSettings Configuration { get; private set; }

        public Plugin()
        {
            Name = "Bundle Backuper";
            Description = "Branch Bundle Backuping";
        }

        IGitUICommands IFactory<IGitUICommands>.Create()
            => commands ?? throw Ensure.Exception.NotSupported("Plugin is not yet registered.");

        public override bool Execute(GitUIBaseEventArgs gitUiCommands)
            => true;

        public override IEnumerable<ISetting> GetSettings()
            => Configuration;

        private MenuStripEx FindMainMenu(IGitUICommands commands)
        {
            FormBrowse form = (FormBrowse)commands.BrowseRepo;
            if (form != null)
            {
                MenuStripEx mainMenu = form.Controls.OfType<MenuStripEx>().FirstOrDefault();
                return mainMenu;
            }

            return null;
        }

        private BundleListMenuItem FindMainMenuItem(IGitUICommands commands, MenuStripEx mainMenu = null)
        {
            if (mainMenu == null)
                mainMenu = FindMainMenu(commands);

            if (mainMenu == null)
                return null;

            return mainMenu.Items.OfType<BundleListMenuItem>().FirstOrDefault();
        }

        public override void Register(IGitUICommands commands)
        {
            base.Register(commands);

            this.commands = commands;
            Configuration = new PluginSettings(Settings);

            if (commands.GitModule.IsValidGitWorkingDir())
            {
                MenuStripEx mainMenu = FindMainMenu(commands);
                if (mainMenu != null && FindMainMenuItem(commands, mainMenu) == null)
                {
                    var provider = new FileSystemBundleProvider(Configuration);
                    var service = new GitUiCommandsBundleService(this, new DefaultBundleNameProvider(Configuration, this));
                    var preferedExecutor = new PreferedCommandAfterBundleExecutor(Configuration, this, service);
                    disposables.Add(preferedExecutor);

                    mainMenu.Items.Add(new BundleListMenuItem(provider, service, service));
                }
            }
        }

        public override void Unregister(IGitUICommands commands)
        {
            base.Unregister(commands);

            MenuStripEx mainMenu = FindMainMenu(commands);
            if (mainMenu != null)
            {
                BundleListMenuItem mainMenuItem = FindMainMenuItem(commands, mainMenu);
                if (mainMenuItem != null)
                {
                    mainMenu.Items.Remove(mainMenuItem);
                    mainMenuItem.Dispose();
                }
            }

            foreach (IDisposable disposable in disposables)
                disposable.Dispose();

            disposables.Clear();
        }
    }
}
