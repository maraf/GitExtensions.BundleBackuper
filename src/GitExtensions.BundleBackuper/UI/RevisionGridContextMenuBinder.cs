using GitCommands;
using GitExtensions.BundleBackuper.Services;
using GitUI;
using GitUIPluginInterfaces;
using Neptuo;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace GitExtensions.BundleBackuper.UI
{
    /// <summary>
    /// A binder for revisition grid context menu.
    /// Adds there items for backuping.
    /// </summary>
    public class RevisionGridContextMenuBinder : IDisposable
    {
        private readonly RevisionGridControl revisionGrid;
        private readonly ContextMenuStrip contextMenu;
        private readonly IGitBundleFactory bundleFactory;

        private ToolStripSeparator separator;
        private ManualBackupButton button;

        public RevisionGridContextMenuBinder(RevisionGridControl revisionGrid, ContextMenuStrip contextMenu, IGitBundleFactory bundleFactory)
        {
            Ensure.NotNull(revisionGrid, "revisionGrid");
            Ensure.NotNull(contextMenu, "contextMenu");
            Ensure.NotNull(bundleFactory, "bundleFactory");
            this.revisionGrid = revisionGrid;
            this.contextMenu = contextMenu;
            this.bundleFactory = bundleFactory;

            contextMenu.Opening += OnRevisionGridContextMenuOpening;
            contextMenu.Closing += OnRevisionGridContextMenuClosing;
        }

        private void OnRevisionGridContextMenuOpening(object sender, CancelEventArgs e)
        {
            IReadOnlyList<GitRevision> revisions = revisionGrid.GetSelectedRevisions();
            foreach (GitRevision revision in revisions)
            {
                foreach (IGitRef reference in revision.Refs)
                {
                    if (!reference.IsRemote)
                    {
                        contextMenu.Items.Add(separator = new ToolStripSeparator());
                        contextMenu.Items.Add(button = new ManualBackupButton(bundleFactory, reference.CompleteName));
                        return;
                    }
                }
            }
        }

        private void OnRevisionGridContextMenuClosing(object sender, ToolStripDropDownClosingEventArgs e)
        {
            if (button != null)
            {
                contextMenu.Items.Remove(button);
                button = null;
            }

            if (separator != null)
            {
                contextMenu.Items.Remove(separator);
                separator = null;
            }
        }

        public void Dispose()
        {
            contextMenu.Opening -= OnRevisionGridContextMenuOpening;
            contextMenu.Closing -= OnRevisionGridContextMenuClosing;
        }
    }
}
