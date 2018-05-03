﻿using GitExtensions.BundleBackuper.Services;
using GitUIPluginInterfaces;
using Neptuo;
using Neptuo.Activators;
using Neptuo.Diagnostics;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace GitExtensions.BundleBackuper.UI
{
    public class AddButton : ToolStripMenuItem
    {
        private readonly IGitBundleMapper mapper;
        private readonly IFactory<IGitUICommands> commandsFactory;
        private readonly IBundleNameProvider nameProvider;

        internal AddButton(IGitBundleMapper mapper, IFactory<IGitUICommands> commandsFactory, IBundleNameProvider nameProvider)
            : base("Backup current branch")
        {
            Ensure.NotNull(mapper, "mapper");
            Ensure.NotNull(commandsFactory, "commandsFactory");
            Ensure.NotNull(nameProvider, "nameProvider");
            this.mapper = mapper;
            this.commandsFactory = commandsFactory;
            this.nameProvider = nameProvider;

            Click += OnClicked;
        }

        private void OnClicked(object sender, EventArgs e)
        {
            Stopwatch sw = new Stopwatch();
            sw.Start();

            IGitUICommands commands = commandsFactory.Create();

            int i = 0;
            while (true)
            {
                string branches = commands.GitCommand($"branch -r --contains HEAD~{i}");
                Console.WriteLine(branches);

                if (!String.IsNullOrWhiteSpace(branches))
                {
                    if (i > 1)
                    {
                        string commitId = commands.GitCommand($"rev-parse HEAD~{i}").Trim();
                        if (!String.IsNullOrWhiteSpace(commitId))
                        {
                            Bundle bundle = nameProvider.Get();
                            commands.StartGitCommandProcessDialog($"bundle create {bundle.FilePath} {commitId}..HEAD");
                        }
                    }

                    break;
                }
                else if (i > 1000)
                {
                    break;
                }

                i++;
            }

            sw.Stop();
            MessageBox.Show(sw.Elapsed.TotalSeconds.ToString());
        }
    }
}
