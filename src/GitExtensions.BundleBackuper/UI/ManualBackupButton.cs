﻿using GitCommands;
using GitExtensions.BundleBackuper.Properties;
using GitExtensions.BundleBackuper.Services;
using GitUI;
using Neptuo;
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
    /// Manual backup button.
    /// </summary>
    public class ManualBackupButton : ToolStripMenuItem
    {
        private readonly IGitBundleFactory bundleFactory;
        private readonly string referenceName;

        private ManualBackupButton(string text, IGitBundleFactory bundleFactory)
            : base(text, Resources.BackupIcon)
        {
            Ensure.NotNull(bundleFactory, "bundleFactory");
            this.bundleFactory = bundleFactory;

            Click += OnClicked;
        }

        internal ManualBackupButton(IGitBundleFactory bundleFactory)
            : this("&Backup current branch", bundleFactory)
        {
        }

        internal ManualBackupButton(IGitBundleFactory bundleFactory, string referenceName)
            : this("&Backup branch", bundleFactory)
        {
            Ensure.NotNullOrEmpty(referenceName, "referenceName");
            this.referenceName = referenceName;
        }

        private async void OnClicked(object sender, EventArgs e)
        {
            if (referenceName == null)
                await bundleFactory.CreateAsync();
            else
                await bundleFactory.CreateAsync(referenceName);
        }
    }
}
