using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GitExtensions.BundleBackuper.Services
{
    /// <summary>
    /// Provides notifications about bundle backup creation operations.
    /// </summary>
    public interface IGitBundleFactoryNotification
    {
        /// <summary>
        /// Executed before backup creation.
        /// Can be cancelled.
        /// </summary>
        event Action<Bundle, CancelEventArgs> Creating;

        /// <summary>
        /// Executed backup bundle was created.
        /// </summary>
        event Action<Bundle> Created;
    }
}
