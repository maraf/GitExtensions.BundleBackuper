﻿using GitUIPluginInterfaces;
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
                AfterRemoveRemoteProperty,
                IsBackupPathCopiedToClipboardProperty
            };
        }

        public IEnumerator<ISetting> GetEnumerator()
            => properties.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator()
            => GetEnumerator();

        #endregion
    }
}
