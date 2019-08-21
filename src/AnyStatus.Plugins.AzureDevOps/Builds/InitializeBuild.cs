using AnyStatus.API;
using AnyStatus.Plugins.AzureDevOps.API;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace AnyStatus.Plugins.AzureDevOps.Builds
{
    public class InitializeBuild : IInitialize<BuildWidget>
    {
        public async Task Handle(InitializeRequest<BuildWidget> request, CancellationToken cancellationToken)
        {
            var api = new AzureDevOpsApi(request.DataContext.ConnectionSettings);

            var response = await api.GetBuildDefinitionsAsync(request.DataContext.Project, request.DataContext.Definition, 1, cancellationToken);

            var buildDefinition = response.Value.FirstOrDefault();

            if (buildDefinition == null)
            {
                throw new Exception($"Build definition \"{request.DataContext.Definition}\" was not found.");
            }

            request.DataContext.DefinitionId = buildDefinition.Id;
            request.DataContext.URL = buildDefinition.Links["web"]["href"];
        }
    }
}