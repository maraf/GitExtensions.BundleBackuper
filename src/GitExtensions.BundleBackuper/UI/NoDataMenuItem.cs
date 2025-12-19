using System.Windows.Forms;

namespace GitExtensions.BundleBackuper.UI
{
    /// <summary>
    /// A menu item with no data message.
    /// </summary>
    public class NoDataMenuItem : ToolStripMenuItem
    {
        public NoDataMenuItem()
            : base("Nothing found...")
        {
            Enabled = false;
        }
    }
}
