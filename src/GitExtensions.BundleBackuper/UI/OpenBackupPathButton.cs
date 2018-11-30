﻿using Neptuo;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
            : base("Open backup path")
        {
            Ensure.NotNull(settings, "settings");
            this.settings = settings;

            Click += OnClicked;
        }

        private void OnClicked(object sender, EventArgs e)
            => Process.Start("explorer", settings.BackupPath);
    }
}
