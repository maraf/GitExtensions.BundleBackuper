using System;

namespace GitExtensions.BundleBackuper.Services
{
    /// <summary>
    /// Provides notifications about bundle backup mapping operations.
    /// </summary>
    public interface IGitBundleMapperNotification
    {
        /// <summary>
        /// Executed backup bundle was mapped as remote.
        /// </summary>
        event Action<Bundle> Added;

        /// <summary>
        /// Executed backup bundle was removed as remote.
        /// </summary>
        event Action<Bundle> Removed;
    }
}
