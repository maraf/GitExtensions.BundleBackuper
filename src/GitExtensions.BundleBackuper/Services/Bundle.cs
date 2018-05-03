using Neptuo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GitExtensions.BundleBackuper.Services
{
    /// <summary>
    /// Describes backup bundle.
    /// </summary>
    public class Bundle
    {
        /// <summary>
        /// Gets a bundle name.
        /// </summary>
        public string Name { get; private set; }

        /// <summary>
        /// Gets a path to bundle file.
        /// </summary>
        public string FilePath { get; private set; }

        /// <summary>
        /// Creates a new instance.
        /// </summary>
        /// <param name="name">A bundle name.</param>
        /// <param name="filePath">A path to bundle file.</param>
        public Bundle(string name, string filePath)
        {
            Ensure.NotNull(name, "name");
            Ensure.NotNull(filePath, "filePath");
            Name = name;
            FilePath = filePath;
        }
    }
}
