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
        private readonly IBundleProvider provider;
        private readonly IGitUICommands commands;

        internal BundleToolStripMenuItem(IBundleProvider provider, IGitUICommands commands)
        {
            Ensure.NotNull(provider, "provider");
            Ensure.NotNull(commands, "commands");
            this.provider = provider;
            this.commands = commands;
            Text = "Bundles";
            DropDownOpening += OnDropDownOpening;
            DropDownItemClicked += OnDropDownItemClicked;
        }

        private void OnDropDownOpening(object sender, EventArgs e)
        {
            DropDown.Items.Clear();

            {

            currentBundles = await provider.EnumerateAsync();
            foreach (Bundle bundle in currentBundles)
            {
                DropDown.Items.Add(new ToolStripMenuItem(bundle.Name)
                {
                    Tag = bundle,
                    Checked = commands.GitModule.GetRemotes().Contains(bundle.Name)
                });
            }
        }

        private void OnDropDownItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {
            if (e.ClickedItem is ToolStripMenuItem target && target.Tag is Bundle bundle)
            {
                if (target.Checked)
                {
                    commands.GitModule.RemoveRemote(bundle.Name);
                    //commands.GitCommand("fetch --all");
                    //commands.StartPullDialog();
                    commands.StartGitCommandProcessDialog("fetch --all");
                    commands.RepoChangedNotifier.Notify();
                }
                else
                {
                    if (!String.IsNullOrEmpty(bundle.FilePath) && File.Exists(bundle.FilePath))
                    {
                        commands.GitModule.AddRemote(bundle.Name, bundle.FilePath);
                        //commands.GitCommand("fetch --all");
                        //commands.StartPullDialog();
                        commands.StartGitCommandProcessDialog("fetch --all");
                        commands.RepoChangedNotifier.Notify();
                    }
                    else
                    {
                        MessageBox.Show($"File '{bundle.FilePath}' doesn't exist or is not accessible.");
                    }
                }
            }
        }
    }
}
