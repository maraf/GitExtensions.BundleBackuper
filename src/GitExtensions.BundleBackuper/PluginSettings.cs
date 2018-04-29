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
        public static StringSetting BackupPathProperty { get; } = new StringSetting("Backup Path", "Bundle Backup Path", null);
        public static StringSetting BackupTemplateProperty { get; } = new StringSetting("Backup Template", "Bundle Name Template", "{Branch.Name}.bundle");
        public static StringSetting AfterAddRemoteProperty { get; } = new StringSetting("After Add Remote", "Command to run after remote is added ({0} = remote name)", "fetch --progress \"{0}\"");
        public static StringSetting AfterRemoveRemoteProperty { get; } = new StringSetting("After Add Remote", "Command to run after remote is removed ({0} = remote name)", "fetch --all");

        private readonly ISettingsSource source;

        public string BackupPath => source.GetValue(BackupPathProperty.Name, BackupPathProperty.DefaultValue, t => t);
        public string BackupTemplate => source.GetValue(BackupTemplateProperty.Name, BackupTemplateProperty.DefaultValue, t => t);
        public string AfterAddRemote => source.GetValue(AfterAddRemoteProperty.Name, AfterAddRemoteProperty.DefaultValue, t => t);
        public string AfterRemoveRemote => source.GetValue(AfterRemoveRemoteProperty.Name, AfterRemoveRemoteProperty.DefaultValue, t => t);

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
                BackupPathProperty,
                BackupTemplateProperty,
                AfterAddRemoteProperty,
                AfterRemoveRemoteProperty
            };
        }

        public IEnumerator<ISetting> GetEnumerator()
            => properties.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator()
            => GetEnumerator();

        #endregion
    }
}
