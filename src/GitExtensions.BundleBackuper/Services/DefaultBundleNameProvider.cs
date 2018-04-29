using GitUIPluginInterfaces;
using Neptuo;
using Neptuo.Text.Tokens;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GitExtensions.BundleBackuper.Services
{
    internal class DefaultBundleNameProvider : IBundleNameProvider
    {
        public static class Token
        {
            public const string BranchName = "Branch.Name";
        }

        private readonly PluginSettings settings;
        private readonly IGitUICommands commands;

        public DefaultBundleNameProvider(PluginSettings settings, IGitUICommands commands)
        {
            Ensure.NotNull(settings, "settings");
            Ensure.NotNull(commands, "commands");
            this.settings = settings;
            this.commands = commands;
        }

        public string Get()
        {
            TokenWriter writer = new TokenWriter(settings.BackupTemplate);
            return writer.Format(token =>
            {
                // TODO: Map tokens to actual values.
                if (token == Token.BranchName)
                    return null;

                return token;
            });
        }
    }
}
