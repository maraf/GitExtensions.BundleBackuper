namespace GitExtensions.BundleBackuper.Services
{
    /// <summary>
    /// A service for mapping bundles to current repository.
    /// </summary>
    public interface IGitBundleMapper
    {
        /// <summary>
        /// Returns <c>true</c> if <paramref name="bundle"/> is mapped as remote.
        /// </summary>
        /// <param name="bundle">A bundle to test.</param>
        /// <returns><c>true</c> if <paramref name="bundle"/> is mapped as remote; <c>false</c> otherwise.</returns>
        bool Has(Bundle bundle);

        /// <summary>
        /// Maps <paramref name="bundle"/> (if not already) as remote.
        /// </summary>
        /// <param name="bundle">A bundle to map.</param>
        void Add(Bundle bundle);

        /// <summary>
        /// Removes remote mapping for <paramref name="bundle"/> (if currently mapped).
        /// </summary>
        /// <param name="bundle">A bundle to remote.</param>
        void Remove(Bundle bundle);
    }
}
