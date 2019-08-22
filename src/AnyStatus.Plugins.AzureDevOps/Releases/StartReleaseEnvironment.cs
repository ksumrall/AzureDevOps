using System;
using AnyStatus.API;
using AnyStatus.Plugins.AzureDevOps.API;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace AnyStatus.Plugins.AzureDevOps.Releases
{
    public class StartReleaseEnvironment : IStart<ReleaseEnvironmentWidget>
    {
        private readonly IDialogService _dialogService;

        public StartReleaseEnvironment(IDialogService dialogService)
        {
            _dialogService = dialogService;
        }

        public async Task Handle(StartRequest<ReleaseEnvironmentWidget> request, CancellationToken cancellationToken)
        {
            var dialog = new ConfirmationDialog($"Are you sure you want to start {request.DataContext.Name}?");

            if (_dialogService.ShowDialog(dialog) != DialogResult.Yes)
            {
                return;
            }

            if (request.DataContext.Parent is ReleaseWidget parent)
            {
                var api = new AzureDevOpsApi(parent.ConnectionSettings);

                var response = await api.GetReleasesAsync(parent.Project, parent.DefinitionId, 1, cancellationToken).ConfigureAwait(false);

                if (response.Count == 0)
                {
                    throw new Exception("Release not found.");
                }

                var release = response.Value.First();

                await api.DeployAsync(parent.Project, release.Id, request.DataContext.DeploymentId, cancellationToken).ConfigureAwait(false);

                request.DataContext.State = State.Queued;
            }
        }
    }
}
