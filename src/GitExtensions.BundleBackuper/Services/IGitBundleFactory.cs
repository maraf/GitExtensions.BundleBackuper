using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GitExtensions.BundleBackuper.Services
{
    public interface IGitBundleFactory
    {
        Task<Bundle> CreateAsync();
    }
}
