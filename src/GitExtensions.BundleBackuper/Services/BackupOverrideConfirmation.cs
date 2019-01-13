using Neptuo;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace GitExtensions.BundleBackuper.Services
{
    public class BackupOverrideConfirmation : DisposableBase
    {
        private readonly PluginSettings settings;
        private readonly IGitBundleFactoryNotification factoryNotification;
        private readonly IWin32Window wnd;

        internal BackupOverrideConfirmation(PluginSettings settings, IGitBundleFactoryNotification factoryNotification, IWin32Window wnd)
        {
            Ensure.NotNull(settings, "settings");
            Ensure.NotNull(factoryNotification, "factoryNotification");
            Ensure.NotNull(wnd, "wnd");
            this.settings = settings;
            this.factoryNotification = factoryNotification;
            this.wnd = wnd;
            factoryNotification.Creating += OnBackupCreating;
        }

        private void OnBackupCreating(Bundle bundle, CancelEventArgs e)
        {
            if (settings.IsBackupOverrideCofirmable && File.Exists(bundle.FilePath))
            {
                string message = $"Backup file already exists. Do you want to override it?{Environment.NewLine}{Environment.NewLine}{bundle.FilePath}";
                DialogResult result = MessageBox.Show(wnd, message, "Bundle Backuper", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
                if (result == DialogResult.No)
                    e.Cancel = true;
            }
        }
    }
}
