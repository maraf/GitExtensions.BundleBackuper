using GitExtensions.Extensibility.Git;
using GitExtensions.BundleBackuper.Properties;
using GitExtensions.BundleBackuper.Services;
using GitExtensions.BundleBackuper.UI;
using GitUI;
using GitUI.CommandsDialogs;
using Neptuo;
using Neptuo.Activators;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Windows.Forms;
using GitExtensions.Extensibility.Plugins;
using GitExtensions.Extensibility.Settings;

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
            : base(true)
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

            if (commands.Module.IsValidGitWorkingDir())
            {
                var service = new Lazy<GitUiCommandsBundleService>(() => new GitUiCommandsBundleService(this, new DefaultBundleNameProvider(Configuration, this), Configuration));

                MenuStripEx mainMenu = FindMainMenu(commands);
                if (mainMenu != null && FindMainMenuItem(commands, mainMenu) == null)
                {
                    var provider = new FileSystemBundleProvider(Configuration);
                    disposables.Add(new PreferedCommandAfterBundleExecutor(Configuration, this, service.Value));
                    disposables.Add(new CopyPathToClipboardExecutor(Configuration, service.Value));
                    disposables.Add(new BackupOverrideConfirmation(Configuration, service.Value, FindForm(commands)));

                    mainMenu.Items.Add(new BundleListMenuItem(provider, service.Value, service.Value, Configuration));
                }

                if (TryGetCommitContextMenu(commands, out var revisionGrid, out var contextMenu))
                    disposables.Add(new RevisionGridContextMenuBinder(revisionGrid, contextMenu, service.Value));
            }
        }

        private bool TryGetCommitContextMenu(IGitUICommands commands, out RevisionGridControl revisionGrid, out ContextMenuStrip contextMenu)
        {
            FormBrowse form = FindForm(commands);
            if (form != null)
            {
                BindingFlags bindingFlags = BindingFlags.Instance | BindingFlags.NonPublic;
                FieldInfo revisionGridField = typeof(FormBrowse).GetField("RevisionGrid", bindingFlags);
                if (revisionGridField != null)
                {
                    revisionGrid = (RevisionGridControl)revisionGridField.GetValue(form);
                    FieldInfo contextMenuField = typeof(RevisionGridControl).GetField("mainContextMenu", bindingFlags);
                    if (contextMenuField != null)
                    {
                        contextMenu = (ContextMenuStrip)contextMenuField.GetValue(revisionGrid);
                        return true;
                    }
                }
            }

            revisionGrid = null;
            contextMenu = null;
            return false;
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
