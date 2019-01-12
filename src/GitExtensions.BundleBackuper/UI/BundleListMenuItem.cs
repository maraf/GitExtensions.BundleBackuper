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

            DropDown.Items.Add(new ManualBackupButton(bundleFactory));
            DropDown.Items.Add(new OpenBackupPathButton(settings));
        }

        private async void OnDropDownOpening(object sender, EventArgs e)
        {
            foreach (var item in DropDown.Items.OfType<BundleMapMenuItem>().ToList())
                DropDown.Items.Remove(item);

            RemoveLastSeparator();

            if (!provider.IsAvailable())
            {
                SetItemsEnabled(false);
                return;
            }

            DropDown.Items.Add(new ToolStripSeparator());
            DropDown.Items.Add(new LoadingMenuItem());

            SetItemsEnabled(true);
            DropDown.Items.AddRange(await CreateBundleItemsAsync());

            DropDown.Items.Remove(DropDown.Items.OfType<LoadingMenuItem>().First());
            RemoveLastSeparator();
        }

        private void RemoveLastSeparator()
        {
            if (DropDown.Items.Count == 3)
                DropDown.Items.RemoveAt(2);
        }

        private async Task<ToolStripItem[]> CreateBundleItemsAsync()
        {
            return await Task.Run(async () =>
            {
                IReadOnlyCollection<Bundle> currentBundles = await provider.EnumerateAsync().ConfigureAwait(false);
                List<ToolStripItem> newItems = new List<ToolStripItem>(currentBundles.Count + 1);

                foreach (Bundle bundle in currentBundles)
                    newItems.Add(new BundleMapMenuItem(mapper, bundle));

                return newItems.ToArray();
            });
        }

        private void SetItemsEnabled(bool isEnabled)
        {
            for (int i = 0; i < DropDown.Items.Count; i++)
                DropDown.Items[i].Enabled = isEnabled;
        }
    }
}
