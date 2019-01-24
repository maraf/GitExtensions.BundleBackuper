﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GitExtensions.BundleBackuper.Services
{
    /// <summary>
    /// A factory creating new bundles.
    /// </summary>
    public interface IGitBundleFactory
    {
        /// <summary>
        /// Creates a new backup for current branch.
        /// </summary>
        /// <returns>Returns continuation task, when resolved contains newly created bundle.</returns>
        Task<Bundle> CreateAsync();

        /// <summary>
        /// Creates a new backup for reference <paramref name="referenceName"/>.
        /// </summary>
        /// <param name="referenceName">A name of the referenced object.</param>
        /// <returns>Returns continuation task, when resolved contains newly created bundle.</returns>
        Task<Bundle> CreateAsync(string referenceName);
    }
}
