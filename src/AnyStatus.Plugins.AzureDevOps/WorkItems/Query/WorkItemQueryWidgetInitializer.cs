using AnyStatus.API;
using AnyStatus.Plugins.AzureDevOps.API.Contracts;
using RestSharp;
using RestSharp.Authenticators;
using System;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;

namespace AnyStatus.Plugins.AzureDevOps.WorkItems.Query
{
    public class WorkItemQueryWidgetInitializer : IInitialize<WorkItemQueryWidget>
    {
        public async Task Handle(InitializeRequest<WorkItemQueryWidget> request, CancellationToken cancellationToken)
        {
            var restClient = new RestClient(request.DataContext.ConnectionSettings.URL)
            {
                Authenticator = new HttpBasicAuthenticator(string.Empty, request.DataContext.ConnectionSettings.PersonalAccessToken)
            };

            var url = string.Format("{0}/{1}/_apis/wit/queries?$filter={2}&$top=1&api-version=5.1",
                Uri.EscapeDataString(request.DataContext.ConnectionSettings.Organization),
                Uri.EscapeDataString(request.DataContext.Project),
                Uri.EscapeDataString(request.DataContext.Query));

            var queryRequest = new RestRequest(url);

            var queryResponse = await restClient.ExecuteTaskAsync<CollectionResponse<WorkItemQueryReference>>(queryRequest, cancellationToken).ConfigureAwait(false);

            if (queryResponse.StatusCode != HttpStatusCode.OK || queryResponse.Data?.Value == null || !queryResponse.Data.Value.Any())
                throw new Exception("An error occurred while initializing widget.");

            var query = queryResponse.Data.Value.First();

            request.DataContext.QueryId = query.Id;
            request.DataContext.URL = query.Links["html"]["href"];
            request.DataContext.WIQL = query.Links["wiql"]["href"];
        }
    }
}
