using GitUIPluginInterfaces;
using Neptuo;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace GitExtensions.BundleBackuper.UI
{
    public class BundleToolStripMenuItem : ToolStripMenuItem
    {
        private readonly PluginSettings settings;
        private readonly IGitUICommands commands;

        internal BundleToolStripMenuItem(PluginSettings settings, IGitUICommands commands)
        {
            Ensure.NotNull(settings, "settings");
            Ensure.NotNull(commands, "commands");
            this.settings = settings;
            this.commands = commands;
            Text = "Bundles";
            DropDownOpening += OnDropDownOpening;
            DropDownItemClicked += OnDropDownItemClicked;
        }

        private void OnDropDownOpening(object sender, EventArgs e)
        {
            DropDown.Items.Clear();

            IEnumerable<string> filePaths = Directory.EnumerateFiles(settings.BackupPath, "*.bundle", SearchOption.AllDirectories);
            foreach (string filePath in filePaths)
            {
                string fileName = Path.GetFileNameWithoutExtension(filePath);

                DropDown.Items.Add(new ToolStripMenuItem(fileName)
                {
                    Tag = filePath,
                    Checked = commands.GitModule.GetRemotes().Contains(fileName)
                });
            }
        }

        private void OnDropDownItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {
            if (e.ClickedItem is ToolStripMenuItem target)
            {
                string filePath = (string)e.ClickedItem.Tag;
                string fileName = target.Text;
                if (target.Checked)
                {
                    commands.GitModule.RemoveRemote(fileName);
                    //commands.GitCommand("fetch --all");
                    //commands.StartPullDialog();
                    commands.StartGitCommandProcessDialog("fetch --all");
                    commands.RepoChangedNotifier.Notify();
                }
                else
                {
                    if (!String.IsNullOrEmpty(filePath) && File.Exists(filePath))
                    {
                        commands.GitModule.AddRemote(fileName, filePath);
                        //commands.GitCommand("fetch --all");
                        //commands.StartPullDialog();
                        commands.StartGitCommandProcessDialog("fetch --all");
                        commands.RepoChangedNotifier.Notify();
                    }
                    else
                    {
                        MessageBox.Show($"File '{filePath}' doesn't exist or is not accessible.");
                    }
                }
            }
        }
    }
}
