using AnyStatus.API;
using RestSharp;
using RestSharp.Authenticators;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace AnyStatus.Plugins.AzureDevOps.Releases
{
    public class StopReleaseEnvironment : IStop<ReleaseEnvironmentWidget>
    {
        private readonly IDialogService _dialogService;

        public StopReleaseEnvironment(IDialogService dialogService)
        {
            _dialogService = dialogService;
        }

        public async Task Handle(StopRequest<ReleaseEnvironmentWidget> request, CancellationToken cancellationToken)
        {
            if (_dialogService.ShowDialog(new ConfirmationDialog($"Are you sure you want to stop {request.DataContext.Name}?")) != DialogResult.Yes)
            {
                return;
            }

            if (!(request.DataContext.Parent is ReleaseWidget parent))
            {
                throw new Exception("Parent not found.");
            }

            var restClient = new RestClient(parent.ConnectionSettings.URL)
            {
                Authenticator = new HttpBasicAuthenticator("", parent.ConnectionSettings.PersonalAccessToken)
            };

            var lastReleaseRequest = new RestRequest($"{parent.ConnectionSettings.Organization}/{parent.Project}/_apis/release/releases?api-version=5.0");

            lastReleaseRequest.AddParameter("$top", 1);

            lastReleaseRequest.AddParameter("definitionId", parent.DefinitionId);

            var releasesResponse = await restClient.ExecuteTaskAsync<CollectionResponse<Release>>(lastReleaseRequest, cancellationToken).ConfigureAwait(false);

            if (releasesResponse.ErrorException != null)
            {
                throw new Exception("An error occurred while getting last release from Azure DevOps.", releasesResponse.ErrorException);
            }

            if (!releasesResponse.IsSuccessful)
            {
                throw new Exception($"Azure DevOps API Response: {releasesResponse.StatusDescription} ({releasesResponse.StatusCode}).\n{releasesResponse.Content}");
            }

            if (!releasesResponse.Data.Value.Any())
            {
                throw new Exception("Release definition was not released.");
            }

            var release = releasesResponse.Data.Value.First();

            var stopRequest = new RestRequest($"{parent.ConnectionSettings.Organization}/{parent.Project}/_apis/release/releases/{release.Id}/environments/{request.DataContext.DeploymentId}?api-version=5-preview.0", Method.PATCH);

            stopRequest.AddJsonBody(new { status = "canceled" });

            var response = await restClient.ExecuteTaskAsync(stopRequest, cancellationToken).ConfigureAwait(false);

            if (!response.IsSuccessful)
            {
                throw new Exception("An error occurred while stopping deployment.\n" + response.Content);
            }
        }
    }
}
