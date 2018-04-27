using GitUIPluginInterfaces;
using Neptuo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GitExtensions.BundleBackuper.Services
{
    public class PreferedCommandAfterBundleExecutor : DisposableBase
    {
        private readonly PluginSettings settings;
        private readonly IGitUICommands commands;
        private readonly IGitBundleMapperNotification notifications;

        internal PreferedCommandAfterBundleExecutor(PluginSettings settings, IGitUICommands commands, IGitBundleMapperNotification notifications)
        {
            Ensure.NotNull(settings, "settings");
            Ensure.NotNull(commands, "commands");
            Ensure.NotNull(notifications, "notifications");
            this.settings = settings;
            this.commands = commands;
            this.notifications = notifications;

            notifications.Added += OnAdded;
            notifications.Removed += OnRemoved;
        }

        protected override void DisposeManagedResources()
        {
            base.DisposeManagedResources();
            notifications.Added -= OnAdded;
            notifications.Removed -= OnRemoved;
        }

        private void OnAdded(Bundle bundle)
            => RunCommand(settings.AfterAddRemote, bundle);

        private void OnRemoved(Bundle bundle)
            => RunCommand(settings.AfterRemoveRemote, bundle);

        private void RunCommand(string arguments, Bundle bundle)
        {
            if (!String.IsNullOrEmpty(arguments))
            {
                if (arguments.Contains("{0}"))
                    arguments = String.Format(arguments, bundle.Name);

                commands.StartGitCommandProcessDialog(arguments);
                commands.RepoChangedNotifier.Notify();
            }
        }
    }
}
