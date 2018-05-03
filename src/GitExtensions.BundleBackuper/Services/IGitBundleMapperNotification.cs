using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GitExtensions.BundleBackuper.Services
{
    /// <summary>
    /// Provides notifications about bundle backup operations.
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
