using AnyStatus.API;
using RestSharp;
using RestSharp.Authenticators;
using System;
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
            if (_dialogService.ShowDialog(new ConfirmationDialog($"Are you sure you want to start {request.DataContext.Name}?")) != DialogResult.Yes)
            {
                return;
            }

            var restClient = new RestClient(request.DataContext.ConnectionSettings.URL)
            {
                Authenticator = new HttpBasicAuthenticator("", request.DataContext.ConnectionSettings.PersonalAccessToken)
            };

            var startRequest = new RestRequest($"{request.DataContext.ConnectionSettings.Organization}/{request.DataContext.Project}/_apis/build/builds?api-version=5.0", Method.POST);

            startRequest.AddJsonBody(new
            {
                Definition = new
                {
                    Id = request.DataContext.DefinitionId
                }
            });

            var response = await restClient.ExecuteTaskAsync(startRequest, cancellationToken).ConfigureAwait(false);

            if (response.IsSuccessful)
            {
                request.DataContext.State = State.Queued;
            }
            else
            {
                throw new Exception("An error occurred while starting build.\n" + response.Content);
            }
        }
    }
}
