using AnyStatus.API;
using RestSharp;
using RestSharp.Authenticators;
using System;
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
            if (_dialogService.ShowDialog(new ConfirmationDialog($"Are you sure you want to stop {request.DataContext.Name}?")) != DialogResult.Yes)
            {
                return;
            }

            var restClient = new RestClient(request.DataContext.ConnectionSettings.URL)
            {
                Authenticator = new HttpBasicAuthenticator("", request.DataContext.ConnectionSettings.PersonalAccessToken)
            };

            var startRequest = new RestRequest($"{request.DataContext.ConnectionSettings.Organization}/{request.DataContext.Project}/_apis/release/releases?api-version=5.0", Method.POST);

            startRequest.AddJsonBody(new
            {
                definitionId = request.DataContext.DefinitionId
            });

            var response = await restClient.ExecuteTaskAsync(startRequest, cancellationToken).ConfigureAwait(false);

            if (!response.IsSuccessful)
            {
                throw new Exception("An error occurred while starting release.\n" + response.Content);
            }
        }
    }
}
