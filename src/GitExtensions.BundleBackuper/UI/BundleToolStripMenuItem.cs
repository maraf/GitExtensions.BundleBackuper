using GitExtensions.BundleBackuper.Services;
using GitUIPluginInterfaces;
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
    public class BundleToolStripMenuItem : ToolStripMenuItem
    {
        private readonly IBundleProvider provider;
        private readonly IGitBundleMapper mapper;

        private readonly ToolStripTextBox searchBox;
        private IEnumerable<Bundle> currentBundles;

        internal BundleToolStripMenuItem(IBundleProvider provider, IGitBundleMapper mapper)
        {
            Ensure.NotNull(provider, "provider");
            Ensure.NotNull(mapper, "mapper");
            this.provider = provider;
            this.mapper = mapper;

            Text = "Bundles";
            DropDownOpening += OnDropDownOpening;
            DropDownOpened += OnDropDownOpened;
            DropDownItemClicked += OnDropDownItemClicked;

            searchBox = new ToolStripTextBox()
            {
                AutoSize = false,
                Width = 200,
                ToolTipText = "Search bundles...",
            };
            searchBox.TextChanged += OnSearchBoxTextChanged;
            DropDown.Items.Add(searchBox);
        }

        private void OnSearchBoxTextChanged(object sender, EventArgs e)
        {
            for (int i = 1; i < DropDown.Items.Count; i++)
            {
                if (DropDown.Items[i] is ToolStripMenuItem item && item.Tag is Bundle bundle)
                    item.Visible = bundle.Name.Contains(searchBox.Text);
            }
        }

        private async void OnDropDownOpening(object sender, EventArgs e)
        {
            while (DropDown.Items.Count > 1)
                DropDown.Items.RemoveAt(1);

            currentBundles = await provider.EnumerateAsync();
            foreach (Bundle bundle in currentBundles)
            {
                DropDown.Items.Add(new ToolStripMenuItem(bundle.Name)
                {
                    Tag = bundle,
                    Checked = mapper.Has(bundle)
                });
            }
        }

        private void OnDropDownOpened(object sender, EventArgs e)
        {
            searchBox.Text = string.Empty;
            searchBox.Focus();
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
