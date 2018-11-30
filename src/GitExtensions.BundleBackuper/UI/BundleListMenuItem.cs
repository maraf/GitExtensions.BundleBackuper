using GitExtensions.BundleBackuper.Services;
using Neptuo;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace GitExtensions.BundleBackuper.UI
{
    /// <summary>
    /// Main menu item.
    /// </summary>
    public class BundleListMenuItem : ToolStripMenuItem
    {
        private readonly IBundleProvider provider;
        private readonly IGitBundleMapper mapper;

        internal BundleListMenuItem(IBundleProvider provider, IGitBundleMapper mapper, IGitBundleFactory bundleFactory, PluginSettings settings)
        {
            Ensure.NotNull(provider, "provider");
            Ensure.NotNull(mapper, "mapper");
            this.provider = provider;
            this.mapper = mapper;

            Text = "Bundles";
            DropDownOpening += OnDropDownOpening;
            DropDownItemClicked += OnDropDownItemClicked;

            DropDown.Items.Add(new ManualBackupButton(bundleFactory));
            DropDown.Items.Add(new OpenBackupPathButton(settings));
        }

        private async void OnDropDownOpening(object sender, EventArgs e)
        {
            foreach (var item in DropDown.Items.OfType<ToolStripItem>().Where(i => i.Tag is Bundle).ToList())
                DropDown.Items.Remove(item);

            if (DropDown.Items.Count == 3)
                DropDown.Items.RemoveAt(2);

            IEnumerable<Bundle> currentBundles = await provider.EnumerateAsync();
            foreach (Bundle bundle in currentBundles)
            {
                if (DropDown.Items.Count <= 2)
                    DropDown.Items.Add(new ToolStripSeparator());

                DropDown.Items.Add(new ToolStripMenuItem(bundle.Name)
                {
                    Tag = bundle,
                    Checked = mapper.Has(bundle)
                });
            }
        }

        private void OnDropDownItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {
            if (e.ClickedItem is ToolStripMenuItem target && target.Tag is Bundle bundle)
            {
                if (target.Checked)
                {
                    mapper.Remove(bundle);
                }
                else
                {
                    if (!String.IsNullOrEmpty(bundle.FilePath) && File.Exists(bundle.FilePath))
                        mapper.Add(bundle);
                    else
                        MessageBox.Show($"File '{bundle.FilePath}' doesn't exist or is not accessible.");
                }
            }
        }
    }
}
