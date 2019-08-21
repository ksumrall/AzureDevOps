using AnyStatus.API;
using AnyStatus.Plugins.AzureDevOps.API;
using System.Threading;
using System.Threading.Tasks;

namespace AnyStatus.Plugins.AzureDevOps.Releases
{
    public class StartRelease : IStart<ReleaseWidget>
    {
        private readonly IDialogService _dialogService;

        public StartRelease(IDialogService dialogService)
        {
            _dialogService = dialogService;
        }

        public async Task Handle(StartRequest<ReleaseWidget> request, CancellationToken cancellationToken)
        {
            var dialog = new ConfirmationDialog($"Are you sure you want to create a new release of {request.DataContext.Name}?");

            if (_dialogService.ShowDialog(dialog) != DialogResult.Yes)
            {
                return;
            }

            var api = new AzureDevOpsApi(request.DataContext.ConnectionSettings);

            await api.CreateReleaseAsync(request.DataContext.Project, request.DataContext.DefinitionId, cancellationToken).ConfigureAwait(false);
        }
    }
}
