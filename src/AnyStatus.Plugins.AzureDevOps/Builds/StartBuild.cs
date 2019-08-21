using AnyStatus.API;
using AnyStatus.Plugins.AzureDevOps.API;
using System.Threading;
using System.Threading.Tasks;

namespace AnyStatus.Plugins.AzureDevOps.Builds
{
    public class StartBuild : IStart<BuildWidget>
    {
        private readonly IDialogService _dialogService;

        public StartBuild(IDialogService dialogService)
        {
            _dialogService = dialogService;
        }

        public async Task Handle(StartRequest<BuildWidget> request, CancellationToken cancellationToken)
        {
            var dialog = new ConfirmationDialog($"Are you sure you want to start {request.DataContext.Name}?");

            if (_dialogService.ShowDialog(dialog) != DialogResult.Yes)
            {
                return;
            }

            var api = new AzureDevOpsApi(request.DataContext.ConnectionSettings);

            await api.QueueBuildAsync(request.DataContext.Project, request.DataContext.DefinitionId, cancellationToken).ConfigureAwait(false);

            request.DataContext.State = State.Queued;
        }
    }
}
