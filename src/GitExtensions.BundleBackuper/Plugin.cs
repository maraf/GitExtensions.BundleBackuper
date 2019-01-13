using GitExtensions.BundleBackuper.Properties;
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
using System.ComponentModel.Composition;
using System.Diagnostics;
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
    [Export(typeof(IGitPlugin))]
    public class Plugin : GitPluginBase, IGitPluginForRepository, IFactory<GitUICommands>
    {
        private readonly List<IDisposable> disposables = new List<IDisposable>();
        private GitUICommands commands;

        internal PluginSettings Configuration { get; private set; }

        public Plugin()
        {
            Name = "Bundle Backuper";
            Description = "Branch Bundle Backuping";
            Icon = Resources.Icon;
        }

        GitUICommands IFactory<GitUICommands>.Create()
            => commands ?? throw Ensure.Exception.NotSupported("Plugin is not yet registered.");

        public override bool Execute(GitUIEventArgs gitUiCommands)
        {
            Process.Start("https://github.com/maraf/GitExtensions.BundleBackuper");
            return true;
        }

        public override IEnumerable<ISetting> GetSettings()
            => Configuration;

        private MenuStripEx FindMainMenu(IGitUICommands commands)
        {
            FormBrowse form = FindForm(commands);
            if (form != null)
            {
                MenuStripEx mainMenu = form.Controls.OfType<MenuStripEx>().FirstOrDefault();
                return mainMenu;
            }

            return null;
        }

        private FormBrowse FindForm(IGitUICommands commands)
        {
            return (FormBrowse)((GitUICommands)commands).BrowseRepo;
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

            this.commands = (GitUICommands)commands;
            Configuration = new PluginSettings(Settings);

            if (commands.GitModule.IsValidGitWorkingDir())
            {
                MenuStripEx mainMenu = FindMainMenu(commands);
                if (mainMenu != null && FindMainMenuItem(commands, mainMenu) == null)
                {
                    var provider = new FileSystemBundleProvider(Configuration);
                    var service = new GitUiCommandsBundleService(this, new DefaultBundleNameProvider(Configuration, this));
                    disposables.Add(new PreferedCommandAfterBundleExecutor(Configuration, this, service));
                    disposables.Add(new CopyPathToClipboardExecutor(Configuration, service));
                    disposables.Add(new BackupOverrideConfirmation(Configuration, service, FindForm(commands)));

                    mainMenu.Items.Add(new BundleListMenuItem(provider, service, service, Configuration));
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
