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

        public bool IsAvailable()
            => Directory.Exists(settings.BackupPath);

        public Task<IReadOnlyCollection<Bundle>> EnumerateAsync()
        {
            return Task.Run<IReadOnlyCollection<Bundle>>(() =>
            {
                IEnumerable<string> filePaths = Directory.EnumerateFiles(settings.BackupPath, "*.bundle", SearchOption.AllDirectories);
                List<Bundle> result = new List<Bundle>();
                foreach (string filePath in filePaths)
                {
                    string fileName = Path.GetFileNameWithoutExtension(filePath);
                    result.Add(new Bundle(fileName, filePath));
                }

                result.Sort((b1, b2) => b1.Name.CompareTo(b2.Name));
                return result;
            });
        }
    }
}
