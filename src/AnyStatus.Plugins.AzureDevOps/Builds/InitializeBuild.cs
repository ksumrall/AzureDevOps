using AnyStatus.API;
using RestSharp;
using RestSharp.Authenticators;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AnyStatus.Plugins.AzureDevOps.Common;

namespace AnyStatus.Plugins.AzureDevOps.Builds
{
    public class InitializeBuild : IInitialize<BuildWidget>
    {
        public async Task Handle(InitializeRequest<BuildWidget> request, CancellationToken cancellationToken)
        {
            var restClient = new RestClient(request.DataContext.ConnectionSettings.URL)
            {
                Authenticator = new HttpBasicAuthenticator("", request.DataContext.ConnectionSettings.PersonalAccessToken)
            };

            var buildDefinitionsRequest = new RestRequest($"{request.DataContext.ConnectionSettings.Organization}/{request.DataContext.Project}/_apis/build/definitions");

            buildDefinitionsRequest.AddParameter("$top", 1);
            buildDefinitionsRequest.AddParameter("name", request.DataContext.Definition);

            var buildDefinitionsResponse = await restClient.ExecuteTaskAsync<CollectionResponse<BuildDefinition>>(buildDefinitionsRequest, cancellationToken).ConfigureAwait(false);

            if (buildDefinitionsResponse.ErrorException != null)
            {
                throw new Exception("An error occurred while getting build definition from Azure DevOps.", buildDefinitionsResponse.ErrorException);
            }

            var buildDefinition = buildDefinitionsResponse.Data.Value?.FirstOrDefault();

            if (buildDefinition == null)
            {
                throw new Exception("The build definition was not found.");
            }

            request.DataContext.DefinitionId = buildDefinition.Id;
            request.DataContext.URL = buildDefinition.Links["web"]["href"];
        }
    }
}