using GitExtensions.BundleBackuper.Services;
using Neptuo;
using System;
using System.Collections.Generic;
using System.Linq;
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
        private bool isLoading;

        internal BundleListMenuItem(IBundleProvider provider, IGitBundleMapper mapper, IGitBundleFactory bundleFactory, PluginSettings settings)
        {
            Ensure.NotNull(provider, "provider");
            Ensure.NotNull(mapper, "mapper");
            this.provider = provider;
            this.mapper = mapper;

            Text = "&Bundles";
            DropDownOpening += OnDropDownOpening;

            DropDown.Items.Add(new ManualBackupButton(bundleFactory));
            DropDown.Items.Add(new OpenBackupPathButton(settings));
        }

        private async void OnDropDownOpening(object sender, EventArgs e)
        {
            if (isLoading)
                return;

            try
            {
                isLoading = true;

                foreach (var item in DropDown.Items.OfType<BundleMapMenuItem>().ToList())
                    DropDown.Items.Remove(item);

                NoDataMenuItem noData = DropDown.Items.OfType<NoDataMenuItem>().FirstOrDefault();
                if (noData != null)
                    DropDown.Items.Remove(noData);

                if (DropDown.Items.Count == 3)
                    DropDown.Items.RemoveAt(2);

                DropDown.Items.Add(new ToolStripSeparator());
                int loadingIndex = DropDown.Items.Add(new LoadingMenuItem());

                if (!await provider.IsAvailableAsync())
                {
                    DropDown.Items.RemoveAt(2);
                    DropDown.Items.RemoveAt(2);
                    SetItemsEnabled(false);
                    return;
                }

                SetItemsEnabled(true);
                DropDown.Items.AddRange(await CreateBundleItemsAsync());

                DropDown.Items.RemoveAt(loadingIndex);
            }
            finally
            {
                isLoading = false;
            }
        }

        private async Task<ToolStripItem[]> CreateBundleItemsAsync()
        {
            return await Task.Run(async () =>
            {
                IReadOnlyCollection<Bundle> currentBundles = await provider.EnumerateAsync().ConfigureAwait(false);
                List<ToolStripItem> newItems = new List<ToolStripItem>(Math.Max(currentBundles.Count, 1));

                foreach (Bundle bundle in currentBundles)
                    newItems.Add(new BundleMapMenuItem(mapper, bundle));

                if (newItems.Count == 0)
                    newItems.Add(new NoDataMenuItem());

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
