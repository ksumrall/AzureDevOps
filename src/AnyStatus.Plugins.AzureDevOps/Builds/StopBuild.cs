using AnyStatus.API;
using AnyStatus.Plugins.AzureDevOps.API;
using System.Threading;
using System.Threading.Tasks;

namespace AnyStatus.Plugins.AzureDevOps.Builds
{
    public class StopBuild : IStop<BuildWidget>
    {
        private readonly IDialogService _dialogService;

        public StopBuild(IDialogService dialogService)
        {
            _dialogService = dialogService;
        }

        public async Task Handle(StopRequest<BuildWidget> request, CancellationToken cancellationToken)
        {
            var dialog = new ConfirmationDialog($"Are you sure you want to stop {request.DataContext.Name}?");

            if (_dialogService.ShowDialog(dialog) != DialogResult.Yes)
            {
                return;
            }

            var api = new AzureDevOpsApi(request.DataContext.ConnectionSettings);

            await api.CancelBuildAsync(request.DataContext.Project, request.DataContext.LastBuildId, cancellationToken).ConfigureAwait(false);
        }
    }
}
