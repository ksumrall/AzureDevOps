using AnyStatus.API;
using RestSharp;
using RestSharp.Authenticators;
using System;
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
            if (_dialogService.ShowDialog(new ConfirmationDialog($"Are you sure you want to start {request.DataContext.Name}?")) != DialogResult.Yes)
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

            var lastReleaseResponse = await restClient.ExecuteTaskAsync<CollectionResponse<Release>>(lastReleaseRequest, cancellationToken).ConfigureAwait(false);

            if (lastReleaseResponse.ErrorException != null)
            {
                throw new Exception("An error occurred while getting release from Azure DevOps.", lastReleaseResponse.ErrorException);
            }

            if (!lastReleaseResponse.IsSuccessful)
            {
                throw new Exception($"Azure DevOps API Response: {lastReleaseResponse.StatusDescription} ({lastReleaseResponse.StatusCode}).\n{lastReleaseResponse.Content}");
            }

            if (!lastReleaseResponse.Data.Value.Any())
            {
                throw new Exception("Release definition was not released.");
            }

            var lastRelease = lastReleaseResponse.Data.Value.First();

            var startRequest = new RestRequest($"{parent.ConnectionSettings.Organization}/{parent.Project}/_apis/release/releases/{lastRelease.Id}/environments/{request.DataContext.DeploymentId}?api-version=5-preview.0", Method.PATCH);

            startRequest.AddJsonBody(new
            {
                status = "inProgress"
            });

            var response = await restClient.ExecuteTaskAsync(startRequest, cancellationToken).ConfigureAwait(false);

            if (response.IsSuccessful)
            {
                request.DataContext.State = State.Queued;
            }
            else
            {
                throw new Exception("An error occurred while starting deployment.\n" + response.Content);
            }
        }
    }
}
