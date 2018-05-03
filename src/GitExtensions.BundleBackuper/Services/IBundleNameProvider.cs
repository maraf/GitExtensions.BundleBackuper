using Neptuo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GitExtensions.BundleBackuper.Services
{
    /// <summary>
    /// Bundle name provider.
    /// </summary>
    public interface IBundleNameProvider
    {
        /// <summary>
        /// Gets a bundle model for current branch.
        /// </summary>
        /// <returns>A bundle model for current branch.</returns>
        Bundle Get();
    }
}
