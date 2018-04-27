using GitExtensions.BundleBackuper.Services;
using GitUIPluginInterfaces;
using Neptuo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace GitExtensions.BundleBackuper.UI
{
    public class AddButton : ToolStripMenuItem
    {
        private readonly IGitBundleMapper mapper;
        private readonly IGitUICommands commands;

        public AddButton(IGitBundleMapper mapper)
            : base("Backup current branch")
        {
            Ensure.NotNull(mapper, "mapper");
            //Ensure.NotNull(commands, "commands");
            this.mapper = mapper;
            //this.commands = commands;

            Click += OnClicked;
        }

        private void OnClicked(object sender, EventArgs e)
        {

        }
    }
}
