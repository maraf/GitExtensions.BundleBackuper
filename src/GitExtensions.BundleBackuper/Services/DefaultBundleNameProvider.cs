using GitUIPluginInterfaces;
using Neptuo;
using Neptuo.Activators;
using Neptuo.Text.Tokens;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GitExtensions.BundleBackuper.Services
{
    internal class DefaultBundleNameProvider : IBundleNameProvider
    {
        public static class Token
        {
            public static class Branch
            {
                public const string Name = "Branch.Name";
            }

            public static class WorkingDirectory
            {
                public const string Path = "WorkingDirectory.Path";
                public const string Name = "WorkingDirectory.Name";
            }
        }

        private readonly PluginSettings settings;
        private readonly IFactory<IGitUICommands> commandsFactory;

        public DefaultBundleNameProvider(PluginSettings settings, IFactory<IGitUICommands> commandsFactory)
        {
            Ensure.NotNull(settings, "settings");
            Ensure.NotNull(commandsFactory, "commandsFactory");
            this.settings = settings;
            this.commandsFactory = commandsFactory;
        }

        public Bundle Get()
        {
            TokenWriter writer = new TokenWriter(settings.BackupTemplate);

            IGitUICommands commands = commandsFactory.Create();
            string name = writer.Format(token =>
            {
                // TODO: Map tokens to actual values.
                switch (token)
                {
                    case Token.Branch.Name:
                        return commands.GitModule.GetSelectedBranch();
                    case Token.WorkingDirectory.Path:
                        return commands.GitModule.WorkingDir;
                    case Token.WorkingDirectory.Name:
                        string directoryName = commands.GitModule.WorkingDir;

                        if (directoryName.Last() == Path.DirectorySeparatorChar || directoryName.Last() == Path.AltDirectorySeparatorChar)
                            directoryName = Path.GetDirectoryName(directoryName);

                        return Path.GetFileName(directoryName);
                    default:
                        throw Ensure.Exception.NotSupported($"Not supported token '{token}'.");
                }
            });

            return new Bundle(name, Path.Combine(settings.BackupPath, name));
        }
    }
}
