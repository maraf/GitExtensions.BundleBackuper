using GitExtensions.BundleBackuper.Services;
using GitUIPluginInterfaces;
using Neptuo;
using Neptuo.Activators;
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

        internal BundleToolStripMenuItem(IBundleProvider provider, IGitBundleMapper mapper, IGitBundleFactory bundleFactory)
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
            DropDown.Items.Add(new ToolStripSeparator());
            DropDown.Items.Add(new AddButton(bundleFactory));
        }

        private void OnSearchBoxTextChanged(object sender, EventArgs e)
        {
            string searchText = searchBox.Text.ToLowerInvariant();
            for (int i = 1; i < DropDown.Items.Count; i++)
            {
                if (DropDown.Items[i] is ToolStripMenuItem item && item.Tag is Bundle bundle)
                    item.Visible = bundle.Name.ToLowerInvariant().Contains(searchText);
            }
        }

        private async void OnDropDownOpening(object sender, EventArgs e)
        {
            foreach (var item in DropDown.Items.OfType<ToolStripItem>().Where(i => i.Tag is Bundle).ToList())
                DropDown.Items.Remove(item);

            currentBundles = await provider.EnumerateAsync();
            foreach (Bundle bundle in currentBundles)
            {
                DropDown.Items.Insert(1, new ToolStripMenuItem(bundle.Name)
                {
                    Tag = bundle,
                    Checked = mapper.Has(bundle)
                });
            }
        }

        private void OnDropDownOpened(object sender, EventArgs e)
        {
            searchBox.Text = String.Empty;
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
