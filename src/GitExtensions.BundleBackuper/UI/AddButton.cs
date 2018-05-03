using GitExtensions.BundleBackuper.Services;
using GitUIPluginInterfaces;
using Neptuo;
using Neptuo.Activators;
using Neptuo.Diagnostics;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace GitExtensions.BundleBackuper.UI
{
    public class AddButton : ToolStripMenuItem
    {
        private readonly IGitBundleFactory bundleFactory;

        internal AddButton(IGitBundleFactory bundleFactory)
            : base("Backup current branch")
        {
            Ensure.NotNull(bundleFactory, "bundleFactory");
            this.bundleFactory = bundleFactory;

            Click += OnClicked;
        }

        private async void OnClicked(object sender, EventArgs e)
            => await bundleFactory.CreateAsync();
    }
}
