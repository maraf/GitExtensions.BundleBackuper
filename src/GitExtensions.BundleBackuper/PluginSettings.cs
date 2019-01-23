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
        /// <summary>
        /// Gets a property holding path to backup and restore bundle file.
        /// </summary>
        public static StringSetting BackupPathProperty { get; } = new StringSetting("Backup Path", "Bundle Backup Path", null);

        /// <summary>
        /// Gets a tempate string for generating new bundle name.
        /// Supports tokens defined in <see cref="Services.DefaultBundleNameProvider.Token"/>.
        /// </summary>
        public static StringSetting BackupTemplateProperty { get; } = new StringSetting("Backup Template", "Bundle Name Template", "{Branch.Name}.bundle");

        /// <summary>
        /// Gets a git command arguments for command executed after backup remote is mapped.
        /// </summary>
        public static StringSetting AfterAddRemoteProperty { get; } = new StringSetting("After Add Remote", "Command to run after remote is added ({0} = remote name)", "fetch --progress \"{0}\"");

        /// <summary>
        /// Gets a git command arguments for command executed after backup remote is removed.
        /// </summary>
        public static StringSetting AfterRemoveRemoteProperty { get; } = new StringSetting("After Add Remote", "Command to run after remote is removed ({0} = remote name)", "fetch --all");

        /// <summary>
        /// Gets a <c>true</c> if backup path should be copied to clipboard after backup.
        /// </summary>
        public static BoolSetting IsBackupPathCopiedToClipboardProperty { get; } = new BoolSetting("Copy path to clipboard", "Copy path to clipboard after backup", false);

        /// <summary>
        /// Gets a <c>true</c> if overriding backup must be confirmed.
        /// </summary>
        public static BoolSetting IsBackupOverrideCofirmableProperty { get; } = new BoolSetting("Ask before overriding backup", "Show confirmation message before overriding backup file", false);

        /// <summary>
        /// Gets a name of the remote check for pushed commits.
        /// </summary>
        public static StringSetting RemoteNamesToCheckProperty { get; } = new StringSetting("Remote names to check", "Remote names to check for pushed commits (semicolon separated)", null);

        private readonly ISettingsSource source;

        /// <summary>
        /// Gets current value of <see cref="BackupPathProperty"/>.
        /// </summary>
        public string BackupPath => source.GetValue(BackupPathProperty.Name, BackupPathProperty.DefaultValue, t => t);

        /// <summary>
        /// Gets current value of <see cref="BackupTemplateProperty"/>.
        /// </summary>
        public string BackupTemplate => source.GetValue(BackupTemplateProperty.Name, BackupTemplateProperty.DefaultValue, t => t);

        /// <summary>
        /// Gets current value of <see cref="AfterAddRemoteProperty"/>.
        /// </summary>
        public string AfterAddRemote => source.GetValue(AfterAddRemoteProperty.Name, AfterAddRemoteProperty.DefaultValue, t => t);

        /// <summary>
        /// Gets current value of <see cref="AfterRemoveRemoteProperty"/>.
        /// </summary>
        public string AfterRemoveRemote => source.GetValue(AfterRemoveRemoteProperty.Name, AfterRemoveRemoteProperty.DefaultValue, t => t);

        /// <summary>
        /// Gets current value of <see cref="IsBackupPathCopiedToClipboardProperty"/>.
        /// </summary>
        public bool IsBackupPathCopiedToClipboard => source.GetValue(IsBackupPathCopiedToClipboardProperty.Name, IsBackupPathCopiedToClipboardProperty.DefaultValue, t => Boolean.Parse(t));

        /// <summary>
        /// Gets current value of <see cref="IsBackupOverrideCofirmableProperty"/>.
        /// </summary>
        public bool IsBackupOverrideCofirmable => source.GetValue(IsBackupOverrideCofirmableProperty.Name, IsBackupOverrideCofirmableProperty.DefaultValue, t => Boolean.Parse(t));

        private IReadOnlyCollection<string> remoteNamesToCheck;
        private string remoteNamesToCheckSource;

        /// <summary>
        /// Gets current value of <see cref="RemoteNamesToCheckProperty"/>.
        /// </summary>
        public IReadOnlyCollection<string> RemoteNamesToCheck
        {
            get
            {
                string current = source.GetValue(RemoteNamesToCheckProperty.Name, RemoteNamesToCheckProperty.DefaultValue, t => t);
                if (remoteNamesToCheckSource != current)
                {
                    remoteNamesToCheck = (current ?? String.Empty).Split(';');
                    remoteNamesToCheckSource = current;
                }

                if (remoteNamesToCheck == null)
                    remoteNamesToCheck = Array.Empty<string>();

                return remoteNamesToCheck;
            }
        }

        public PluginSettings(ISettingsSource source)
        {
            Ensure.NotNull(source, "source");
            this.source = source;
        }

        #region IEnumerable<ISetting>

        private static readonly List<ISetting> properties;

        static PluginSettings()
        {
            properties = new List<ISetting>(7)
            {
                BackupPathProperty,
                BackupTemplateProperty,
                AfterAddRemoteProperty,
                AfterRemoveRemoteProperty,
                IsBackupPathCopiedToClipboardProperty,
                IsBackupOverrideCofirmableProperty,
                RemoteNamesToCheckProperty
            };
        }

        public IEnumerator<ISetting> GetEnumerator()
            => properties.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator()
            => GetEnumerator();

        #endregion
    }
}
