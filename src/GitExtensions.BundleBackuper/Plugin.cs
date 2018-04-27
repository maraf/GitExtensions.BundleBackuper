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
        private readonly List<IDisposable> disposables = new List<IDisposable>();

        internal PluginSettings Configuration { get; private set; }

        public Plugin()
        {
            Name = "Bundle Backuper";
            Description = "Branch Bundle Backuping";
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
                {
                    var provider = new FileSystemBundleProvider(Configuration);
                    var mapper = new GitUiCommandsBundleMapper(commands);
                    var preferedExecutor = new PreferedCommandAfterBundleExecutor(Configuration, commands, mapper);
                    disposables.Add(preferedExecutor);

                    mainMenu.Items.Add(new BundleToolStripMenuItem(provider, mapper));
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
