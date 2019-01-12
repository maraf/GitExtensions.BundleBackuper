using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
        { }
    }
}
