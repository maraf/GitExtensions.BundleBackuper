using GitUIPluginInterfaces;
using Neptuo;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GitExtensions.BundleBackuper
{
    internal class PluginSettings : IEnumerable<ISetting>
    {
        public static StringSetting BackupPathProperty { get; } = new StringSetting("Backup Path", "A path where to store and load from bundle backups.", null);

        private readonly ISettingsSource source;

        public PluginSettings(ISettingsSource source)
        {
            Ensure.NotNull(source, "source");
            this.source = source;
        }

        #region IEnumerable<ISetting>

        private static readonly List<ISetting> properties;

        static PluginSettings()
        {
            properties = new List<ISetting>(1)
            {
                BackupPathProperty
            };
        }

        public IEnumerator<ISetting> GetEnumerator()
            => properties.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator()
            => GetEnumerator();

        #endregion
    }
}
