using GitUIPluginInterfaces;
using Neptuo;
using Neptuo.Activators;
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
        private readonly IFactory<IGitUICommands> commandsFactory;
        private readonly IGitBundleMapperNotification notifications;

        internal PreferedCommandAfterBundleExecutor(PluginSettings settings, IFactory<IGitUICommands> commandsFactory, IGitBundleMapperNotification notifications)
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

                IGitUICommands commands = commandsFactory.Create();
                commands.StartGitCommandProcessDialog(arguments);
                commands.RepoChangedNotifier.Notify();
            }
        }
    }
}
