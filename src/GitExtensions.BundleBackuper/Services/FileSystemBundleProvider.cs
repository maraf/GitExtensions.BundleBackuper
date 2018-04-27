using Neptuo;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GitExtensions.BundleBackuper.Services
{
    public class FileSystemBundleProvider : IBundleProvider
    {
        private readonly PluginSettings settings;

        internal FileSystemBundleProvider(PluginSettings settings)
        {
            Ensure.NotNull(settings, "settings");
            this.settings = settings;
        }

        public Task<IEnumerable<Bundle>> EnumerateAsync()
        {
            return Task.Factory.StartNew<IEnumerable<Bundle>>(() =>
            {
                IEnumerable<string> filePaths = Directory.EnumerateFiles(settings.BackupPath, "*.bundle", SearchOption.AllDirectories);
                List<Bundle> result = new List<Bundle>();
                foreach (string filePath in filePaths)
                {
                    string fileName = Path.GetFileNameWithoutExtension(filePath);
                    result.Add(new Bundle(fileName, filePath));
                }

                return result;
            });
        }
    }
}
