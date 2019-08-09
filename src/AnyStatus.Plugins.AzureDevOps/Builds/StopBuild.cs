using AnyStatus.API;
using RestSharp;
using RestSharp.Authenticators;
using System;
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
            if (_dialogService.ShowDialog(new ConfirmationDialog($"Are you sure you want to stop {request.DataContext.Name}?")) != DialogResult.Yes)
            {
                return;
            }

            var restClient = new RestClient(request.DataContext.ConnectionSettings.URL)
            {
                Authenticator = new HttpBasicAuthenticator("", request.DataContext.ConnectionSettings.PersonalAccessToken)
            };

            var stopRequest = new RestRequest($"{request.DataContext.ConnectionSettings.Organization}/{request.DataContext.Project}/_apis/build/builds/{request.DataContext.LastBuildId}?api-version=5.0", Method.PATCH);

            stopRequest.AddJsonBody(new { status = "cancelling" });

            var response = await restClient.ExecuteTaskAsync(stopRequest, cancellationToken).ConfigureAwait(false);

            if (!response.IsSuccessful)
            {
                throw new Exception("An error occurred while stopping build.\n" + response.Content);
            }
        }
    }
}
