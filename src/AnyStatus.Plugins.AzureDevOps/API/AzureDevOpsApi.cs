﻿using AnyStatus.Plugins.AzureDevOps.API.Contracts;
using RestSharp;
using RestSharp.Authenticators;
using System;
using System.Collections.Generic;
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

            _client = new RestClient($"{settings.URL}/{Uri.EscapeDataString(settings.Organization)}")
            {
                Authenticator = new HttpBasicAuthenticator("", settings.PersonalAccessToken)
            };
        }

        private async Task<T> ExecuteAsync<T>(IRestRequest request, CancellationToken cancellationToken) where T : new()
        {
            var response = await _client.ExecuteTaskAsync<T>(request, cancellationToken).ConfigureAwait(false);

            if (!response.IsSuccessful || response.ErrorException != null)
                throw new Exception("An error occurred while requesting data from Azure DevOps.", response.ErrorException);

            return response.Data;
        }

        private async Task ExecuteAsync(IRestRequest request, CancellationToken cancellationToken)
        {
            var response = await _client.ExecuteTaskAsync(request, cancellationToken).ConfigureAwait(false);

            if (!response.IsSuccessful || response.ErrorException != null)
                throw new Exception("An error occurred while sending request to Azure DevOps.", response.ErrorException);
        }

        //Builds

        internal async Task<CollectionResponse<Build>> GetBuildsAsync(string project, int definitionId, int top, CancellationToken cancellationToken)
        {
            var request = new RestRequest($"{Uri.EscapeDataString(project)}/_apis/build/builds?api-version=5.0");

            request.AddParameter("$top", top);
            request.AddParameter("definitions", definitionId);

            return await ExecuteAsync<CollectionResponse<Build>>(request, cancellationToken).ConfigureAwait(false);
        }

        internal async Task<CollectionResponse<BuildDefinition>> GetBuildDefinitionsAsync(string project, string name, int top, CancellationToken cancellationToken)
        {
            var request = new RestRequest($"{Uri.EscapeDataString(project)}/_apis/build/definitions?api-version=5.0");

            request.AddParameter("$top", top);
            request.AddParameter("name", name);

            return await ExecuteAsync<CollectionResponse<BuildDefinition>>(request, cancellationToken).ConfigureAwait(false);
        }

        internal async Task QueueBuildAsync(string project, int definitionId, CancellationToken cancellationToken)
        {
            var request = new RestRequest($"{Uri.EscapeDataString(project)}/_apis/build/builds?api-version=5.0", Method.POST);

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
            var request = new RestRequest($"{Uri.EscapeDataString(project)}/_apis/build/builds/{buildId}?api-version=5.0", Method.PATCH);

            request.AddJsonBody(new { status = "cancelling" });

            await ExecuteAsync(request, cancellationToken).ConfigureAwait(false);
        }

        //Releases

        internal async Task<CollectionResponse<Release>> GetReleasesAsync(string project, int definitionId, int top, CancellationToken cancellationToken)
        {
            var request = new RestRequest($"{Uri.EscapeDataString(project)}/_apis/release/releases?api-version=5.0");

            request.AddParameter("definitionId", definitionId);
            request.AddParameter("$expand", "environments");
            request.AddParameter("$top", top);

            return await ExecuteAsync<CollectionResponse<Release>>(request, cancellationToken).ConfigureAwait(false);
        }

        internal async Task CreateReleaseAsync(string project, int definitionId, CancellationToken cancellationToken)
        {
            var request = new RestRequest($"{Uri.EscapeDataString(project)}/_apis/release/releases?api-version=5.0", Method.POST);

            request.AddJsonBody(new { definitionId });

            await ExecuteAsync(request, cancellationToken).ConfigureAwait(false);
        }

        //Deployments

        internal async Task<CollectionResponse<Deployment>> GetDeploymentsAsync(string project, int definitionId, int top, CancellationToken cancellationToken)
        {
            var request = new RestRequest($"{Uri.EscapeDataString(project)}/_apis/release/deployments");

            request.AddParameter("definitionId", definitionId);
            request.AddParameter("$top", top);
            request.AddParameter("api-version", "5.0");

            return await ExecuteAsync<CollectionResponse<Deployment>>(request, cancellationToken).ConfigureAwait(false);
        }

        internal async Task<CollectionResponse<Deployment>> GetDeploymentsAsync(string project, int definitionId, int definitionEnvironmentId, int top, CancellationToken cancellationToken)
        {
            var request = new RestRequest($"{Uri.EscapeDataString(project)}/_apis/release/deployments");

            request.AddParameter("definitionId", definitionId);
            request.AddParameter("definitionEnvironmentId", definitionEnvironmentId);
            request.AddParameter("$top", top);
            request.AddParameter("api-version", "5.0");

            return await ExecuteAsync<CollectionResponse<Deployment>>(request, cancellationToken).ConfigureAwait(false);
        }

        internal async Task DeployAsync(string project, int releaseId, int deploymentId, CancellationToken cancellationToken)
        {
            var request = new RestRequest($"{Uri.EscapeDataString(project)}/_apis/release/releases/{releaseId}/environments/{deploymentId}?api-version=5-preview.0", Method.PATCH);

            request.AddJsonBody(new { status = "inProgress" });

            await ExecuteAsync(request, cancellationToken).ConfigureAwait(false);
        }

        internal async Task CancelDeploymentAsync(string project, int releaseId, int deploymentId, CancellationToken cancellationToken)
        {
            var request = new RestRequest($"{Uri.EscapeDataString(project)}/_apis/release/releases/{releaseId}/environments/{deploymentId}?api-version=5-preview.0", Method.PATCH);

            request.AddJsonBody(new { status = "canceled" });

            await ExecuteAsync(request, cancellationToken).ConfigureAwait(false);
        }

        //Work Items

        internal async Task<WorkItemQueryResult> QueryWorkItemsAsync(string query, CancellationToken cancellationToken)
        {
            var request = new RestRequest("_apis/wit/wiql?api-version=5.0", Method.POST);

            request.AddJsonBody(new { query });

            return await ExecuteAsync<WorkItemQueryResult>(request, cancellationToken).ConfigureAwait(false);
        }

        internal async Task<WorkItemQueryResult> QueryWorkItemsByIdAsync(string id, CancellationToken cancellationToken)
        {
            var request = new RestRequest($"_apis/wit/wiql/{id}?api-version=5.0");

            return await ExecuteAsync<WorkItemQueryResult>(request, cancellationToken).ConfigureAwait(false);
        }

        internal async Task<CollectionResponse<WorkItem>> GetWorkItemsAsync(List<string> ids, CancellationToken cancellationToken)
        {
            var request = new RestRequest("_apis/wit/workitemsbatch?api-version=5.0", Method.POST);

            request.AddJsonBody(new Dictionary<string, object>
            {
                ["ids"] = ids,
                ["$expand"] = "Links",
                ["fields"] = new[] { "System.Id", "System.Title", "System.WorkItemType", "System.TeamProject" }
            });

            return await ExecuteAsync<CollectionResponse<WorkItem>>(request, cancellationToken).ConfigureAwait(false);
        }

        internal async Task<CollectionResponse<WorkItemQuery>> GetWorkItemQueriesAsync(string project, string filter, int top, CancellationToken cancellationToken)
        {
            var request = new RestRequest($"{Uri.EscapeDataString(project)}/_apis/wit/queries?$filter={Uri.EscapeDataString(filter)}&$top={top}&api-version=5.1");

            return await ExecuteAsync<CollectionResponse<WorkItemQuery>>(request, cancellationToken).ConfigureAwait(false);
        }

        // Repositories

        internal async Task<GitRepository> GetRepositoryAsync(string project, string name, CancellationToken cancellationToken)
        {
            var request = new RestRequest($"{Uri.EscapeDataString(project)}/_apis/git/repositories/{Uri.EscapeDataString(name)}?api-version=5.1");

            return await ExecuteAsync<GitRepository>(request, cancellationToken).ConfigureAwait(false);
        }

        //Pull Requests

        internal async Task<CollectionResponse<GitPullRequest>> GetPullRequestsAsync(string project, string repositoryId, string sourceRefName, string targetRefName, CancellationToken cancellationToken)
        {
            var request = new RestRequest($"{Uri.EscapeDataString(project)}/_apis/git/repositories/{repositoryId}/pullrequests?api-version=5.1");

            if (string.IsNullOrEmpty(sourceRefName) == false)
            {
                request.AddQueryParameter("searchCriteria.sourceRefName", sourceRefName);
            }

            if (!string.IsNullOrEmpty(targetRefName) == false)
            {
                request.AddQueryParameter("searchCriteria.targetRefName", targetRefName);
            }

            return await ExecuteAsync<CollectionResponse<GitPullRequest>>(request, cancellationToken).ConfigureAwait(false);
        }
    }
}
