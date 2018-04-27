using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GitExtensions.BundleBackuper.Services
{
    /// <summary>
    /// A service for mapping bundles to current repository.
    /// </summary>
    public interface IGitBundleMapper
    {
        bool Has(Bundle bundle);
        void Add(Bundle bundle);
        void Remove(Bundle bundle);
    }
}
