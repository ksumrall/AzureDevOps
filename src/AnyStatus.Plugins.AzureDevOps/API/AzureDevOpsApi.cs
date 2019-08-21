using AnyStatus.Plugins.AzureDevOps.Builds;
using AnyStatus.Plugins.AzureDevOps.Common;
using AnyStatus.Plugins.AzureDevOps.Releases;
using RestSharp;
using RestSharp.Authenticators;
using System;
using System.Threading;
using System.Threading.Tasks;

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

        private async Task ExecuteAsync(IRestRequest request, CancellationToken cancellationToken)
        {
            var response = await _client.ExecuteTaskAsync(request, cancellationToken).ConfigureAwait(false);

            if (response.IsSuccessful)
                return;

            const string message = "An error occurred while sending request to Azure DevOps.";

            throw new Exception(message, response.ErrorException);
        }
        //Builds

        internal async Task<CollectionResponse<Build>> GetBuildsAsync(string project, int definitionId, int top, CancellationToken cancellationToken)
        {
            var request = new RestRequest($"{project}/_apis/build/builds?api-version=5.0");

            request.AddParameter("$top", top);
            request.AddParameter("definitions", definitionId);

            return await ExecuteAsync<CollectionResponse<Build>>(request, cancellationToken).ConfigureAwait(false);
        }

        internal async Task<CollectionResponse<BuildDefinition>> GetBuildDefinitionsAsync(string project, string name, int top, CancellationToken cancellationToken)
        {
            var request = new RestRequest($"{project}/_apis/build/definitions?api-version=5.0");

            request.AddParameter("$top", top);
            request.AddParameter("name", name);

            return await ExecuteAsync<CollectionResponse<BuildDefinition>>(request, cancellationToken).ConfigureAwait(false);
        }

        internal async Task QueueBuildAsync(string project, int definitionId, CancellationToken cancellationToken)
        {
            var request = new RestRequest($"{project}/_apis/build/builds?api-version=5.0", Method.POST);

            request.AddJsonBody(new
            {
                Definition = new
                {
                    Id = definitionId
                }
            });

            await ExecuteAsync(request, cancellationToken).ConfigureAwait(false);
        }

        internal async Task CancelBuildAsync(string project, int buildId, CancellationToken cancellationToken)
        {
            var request = new RestRequest($"{project}/_apis/build/builds/{buildId}?api-version=5.0", Method.PATCH);

            request.AddJsonBody(new { status = "cancelling" });

            await ExecuteAsync(request, cancellationToken).ConfigureAwait(false);
        }

        //Releases

        internal async Task<CollectionResponse<Release>> GetReleasesAsync(string project, int definitionId, int top, CancellationToken cancellationToken)
        {
            var request = new RestRequest($"{project}/_apis/release/releases?api-version=5.0");

            request.AddParameter("definitionId", definitionId);
            request.AddParameter("$expand", "environments");
            request.AddParameter("$top", top);

            return await ExecuteAsync<CollectionResponse<Release>>(request, cancellationToken).ConfigureAwait(false);
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
