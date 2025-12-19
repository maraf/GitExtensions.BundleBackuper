using GitUI;
using GitUI.CommandsDialogs;
using Neptuo;
using Neptuo.Activators;
using System;

namespace GitExtensions.BundleBackuper.Services
{
    public class PreferedCommandAfterBundleExecutor : DisposableBase
    {
        private readonly PluginSettings settings;
        private readonly IFactory<GitUICommands> commandsFactory;
        private readonly IGitBundleMapperNotification notifications;

        internal PreferedCommandAfterBundleExecutor(PluginSettings settings, IFactory<GitUICommands> commandsFactory, IGitBundleMapperNotification notifications)
        {
            Ensure.NotNull(settings, "settings");
            Ensure.NotNull(commandsFactory, "commandsFactory");
            Ensure.NotNull(notifications, "notifications");
            this.settings = settings;
            this.commandsFactory = commandsFactory;
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

                GitUICommands commands = commandsFactory.Create();
                commands.StartGitCommandProcessDialog((FormBrowse)commands.BrowseRepo, arguments);
                commands.RepoChangedNotifier.Notify();
            }
        }
    }
}
