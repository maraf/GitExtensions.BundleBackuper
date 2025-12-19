using GitExtensions.BundleBackuper.Services;
using Neptuo;
using System;
using System.IO;
using System.Windows.Forms;

namespace GitExtensions.BundleBackuper.UI
{
    public class BundleMapMenuItem : ToolStripMenuItem
    {
        private readonly IGitBundleMapper mapper;
        private readonly Bundle bundle;

        public BundleMapMenuItem(IGitBundleMapper mapper, Bundle bundle)
        {
            Ensure.NotNull(mapper, "mapper");
            Ensure.NotNull(bundle, "bundle");
            this.mapper = mapper;
            this.bundle = bundle;

            Text = "&" + bundle.Name;
            Checked = mapper.Has(bundle);
            Click += OnClick;
        }

        private void OnClick(object sender, EventArgs e)
        {
            if (Checked)
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
