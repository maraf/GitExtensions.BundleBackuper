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
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace GitExtensions.BundleBackuper
{
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

        public override void Register(IGitUICommands commands)
        {
            base.Register(commands);

            this.commands = commands;
            Configuration = new PluginSettings(Settings);

            FormBrowse form = (FormBrowse)commands.BrowseRepo;
            if (form != null)
            {
                MenuStripEx mainMenu = form.Controls.OfType<MenuStripEx>().FirstOrDefault();
                if (mainMenu != null)
                {
                    if (!mainMenu.Items.OfType<BundleToolStripMenuItem>().Any())
                    {
                        var provider = new FileSystemBundleProvider(Configuration);
                        var service = new GitUiCommandsBundleService(this, new DefaultBundleNameProvider(Configuration, this));
                        var preferedExecutor = new PreferedCommandAfterBundleExecutor(Configuration, this, service);
                        disposables.Add(preferedExecutor);

                        mainMenu.Items.Add(new BundleToolStripMenuItem(provider, service, service));
                    }
                }
            }
        }

        public override void Unregister(IGitUICommands gitUiCommands)
        {
            base.Unregister(gitUiCommands);

            foreach (IDisposable disposable in disposables)
                disposable.Dispose();

            disposables.Clear();
        }
    }
}
