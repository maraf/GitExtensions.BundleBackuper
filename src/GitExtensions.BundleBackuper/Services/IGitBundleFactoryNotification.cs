using System;
using System.Collections.Generic;
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
        /// Executed backup bundle was created.
        /// </summary>
        event Action<Bundle> Created;
    }
}
