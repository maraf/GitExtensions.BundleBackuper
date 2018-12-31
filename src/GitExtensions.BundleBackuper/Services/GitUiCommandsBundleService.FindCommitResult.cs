using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GitExtensions.BundleBackuper.Services
{
    partial class GitUiCommandsBundleService
    {
        /// <summary>
        /// A result of finding base commit to backup.
        /// </summary>
        private enum FindCommitResult
        {
            /// <summary>
            /// A commit was found.
            /// </summary>
            BaseFound,

            /// <summary>
            /// A first commit should also be included (a bundle without base).
            /// </summary>
            WithoutBase,

            /// <summary>
            /// Nothing to backup was found.
            /// </summary>
            NotFound
        }
    }
}
