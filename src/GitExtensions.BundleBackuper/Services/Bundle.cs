using Neptuo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GitExtensions.BundleBackuper.Services
{
    public class Bundle
    {
        public string Name { get; private set; }
        public string FilePath { get; private set; }

        public Bundle(string name, string filePath)
        {
            Ensure.NotNull(name, "name");
            Ensure.NotNull(filePath, "filePath");
            Name = name;
            FilePath = filePath;
        }
    }
}
