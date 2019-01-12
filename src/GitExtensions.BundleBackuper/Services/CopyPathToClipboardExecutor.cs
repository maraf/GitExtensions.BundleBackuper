using Neptuo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace GitExtensions.BundleBackuper.Services
{
    /// <summary>
    /// When a <see cref="PluginSettings.IsBackupPathCopiedToClipboard"/> is set to <c>true</c>, it copies bundle file path clipboard when <see cref="IGitBundleFactoryNotification.Created"/> is raised.
    /// </summary>
    public class CopyPathToClipboardExecutor : DisposableBase
    {
        private readonly PluginSettings settings;
        private readonly IGitBundleFactoryNotification factoryNotification;

        internal CopyPathToClipboardExecutor(PluginSettings settings, IGitBundleFactoryNotification factoryNotification)
        {
            Ensure.NotNull(settings, "settings");
            Ensure.NotNull(factoryNotification, "factoryNotification");
            this.settings = settings;
            this.factoryNotification = factoryNotification;

            this.factoryNotification.Created += OnBundleCreated;
        }

        private void OnBundleCreated(Bundle bundle)
        {
            if (settings.IsBackupPathCopiedToClipboard)
                Clipboard.SetText(bundle.FilePath);
        }

        protected override void DisposeManagedResources()
        {
            base.DisposeManagedResources();

            this.factoryNotification.Created -= OnBundleCreated;
        }
    }
}
