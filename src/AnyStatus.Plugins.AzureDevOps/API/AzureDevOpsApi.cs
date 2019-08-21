using System;
using System.Threading;
using System.Threading.Tasks;
using AnyStatus.Plugins.AzureDevOps.Builds;
using AnyStatus.Plugins.AzureDevOps.Common;
using AnyStatus.Plugins.AzureDevOps.Releases;
using RestSharp;
using RestSharp.Authenticators;

namespace AnyStatus.Plugins.AzureDevOps.API
{
    internal class AzureDevOpsApi
    {
        private readonly IRestClient _client;

        internal AzureDevOpsApi(AzureDevOpsConnectionSettings settings)
        {
            if (settings == null)
                throw new ArgumentNullException(nameof(settings));

            _client = new RestClient($"{settings.URL}/{settings.Organization}")
            {
                Authenticator = new HttpBasicAuthenticator("", settings.PersonalAccessToken)
            };
        }

        private async Task<T> ExecuteAsync<T>(IRestRequest request, CancellationToken cancellationToken) where T : new()
        {
            var response = await _client.ExecuteTaskAsync<T>(request, cancellationToken).ConfigureAwait(false);

            if (response.ErrorException == null)
                return response.Data;

            const string message = "An error occurred while requesting data from Azure DevOps.";

            throw new Exception(message, response.ErrorException);
        }

        internal async Task<CollectionResponse<Release>> GetReleasesAsync(string project, int definitionId, int top, CancellationToken cancellationToken)
        {
            var request = new RestRequest($"{project}/_apis/release/releases?api-version=5.0");

            request.AddParameter("definitionId", definitionId);
            request.AddParameter("$expand", "environments");
            request.AddParameter("$top", top);

            return await ExecuteAsync<CollectionResponse<Release>>(request, cancellationToken).ConfigureAwait(false);
        }

        internal async Task<CollectionResponse<Build>> GetBuildsAsync(string project, int definitionId, int top, CancellationToken cancellationToken)
        {
            var request = new RestRequest($"{project}/_apis/build/builds?api-version=5.0");

            request.AddParameter("definitions", definitionId);
            request.AddParameter("$top", top);

            return await ExecuteAsync<CollectionResponse<Build>>(request, cancellationToken).ConfigureAwait(false);
        }

        internal async Task<CollectionResponse<Deployment>> GetDeploymentsAsync(string project, int definitionId, int top, CancellationToken cancellationToken)
        {
            var request = new RestRequest($"{project}/_apis/release/deployments");

            request.AddParameter("definitionId", definitionId);
            request.AddParameter("$top", top);
            request.AddParameter("api-version", "5.0");

            return await ExecuteAsync<CollectionResponse<Deployment>>(request, cancellationToken).ConfigureAwait(false);
        }
    }
}
