using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GitExtensions.BundleBackuper.Services
{
    /// <summary>
    /// Backup bundle list provider.
    /// </summary>
    public interface IBundleProvider
    {
        /// <summary>
        /// Returns <c>true</c> if storage is available for reading.
        /// </summary>
        /// <returns><c>true</c> if storage is available for reading.</returns>
        Task<bool> IsAvailableAsync();

        /// <summary>
        /// Enumerates all currently available backup bundles.
        /// </summary>
        /// <returns>Enumeration of all currently available backup bundles.</returns>
        Task<IReadOnlyCollection<Bundle>> EnumerateAsync();
    }
}
