using GitUI.Properties;
using Neptuo;
using System;
using System.Diagnostics;
using System.Windows.Forms;

namespace GitExtensions.BundleBackuper.UI
{
    /// <summary>
    /// Open backup path in explorer.
    /// </summary>
    public class OpenBackupPathButton : ToolStripMenuItem
    {
        private readonly PluginSettings settings;

        internal OpenBackupPathButton(PluginSettings settings)
            : base("&Open backup path", Images.BrowseFileExplorer)
        {
            Ensure.NotNull(settings, "settings");
            this.settings = settings;

            Click += OnClicked;
        }

        private void OnClicked(object sender, EventArgs e)
            => Process.Start("explorer", settings.BackupPath);
    }
}
