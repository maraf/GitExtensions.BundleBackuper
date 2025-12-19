using System.Windows.Forms;

namespace GitExtensions.BundleBackuper.UI
{
    /// <summary>
    /// A loading state menu item.
    /// </summary>
    public class LoadingMenuItem : ToolStripMenuItem
    {
        public LoadingMenuItem()
            : base("Loading...")
        {
            Enabled = false;
        }
    }
}
