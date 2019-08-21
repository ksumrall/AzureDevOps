using AnyStatus.API;
using RestSharp;
using RestSharp.Authenticators;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AnyStatus.Plugins.AzureDevOps.Common;

namespace AnyStatus.Plugins.AzureDevOps.Releases
{
    public class InitializeRelease : IInitialize<ReleaseWidget>
    {
        public async Task Handle(InitializeRequest<ReleaseWidget> request, CancellationToken cancellationToken)
        {
            var widget = request.DataContext;

            var client = new RestClient(widget.ConnectionSettings.URL)
            {
                Authenticator = new HttpBasicAuthenticator("", widget.ConnectionSettings.PersonalAccessToken)
            };

            var releaseDefinition = await GetReleaseDefinitionAsync(widget, client, cancellationToken);

            if (releaseDefinition == null)
            {
                throw new Exception("Release definition was not found.");
            }

            widget.DefinitionId = releaseDefinition.Id;
            widget.URL = releaseDefinition.Links["web"]["href"];
        }

        private static async Task<ReleaseDefinition> GetReleaseDefinitionAsync(ReleaseWidget widget, IRestClient restClient, CancellationToken cancellationToken)
        {
            var request = new RestRequest($"{widget.ConnectionSettings.Organization}/{widget.Project}/_apis/release/definitions");

            request.AddParameter("$top", 1);
            request.AddParameter("searchText", widget.Definition);

            var response = await restClient.ExecuteTaskAsync<CollectionResponse<ReleaseDefinition>>(request, cancellationToken).ConfigureAwait(false);

            if (response.ErrorException != null)
                throw new Exception("An error occurred while getting release defintion from Azure DevOps.", response.ErrorException);

            return response.Data.Value?.FirstOrDefault();
        }
    }
}