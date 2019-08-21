using AnyStatus.API;
using RestSharp;
using RestSharp.Authenticators;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AnyStatus.Plugins.AzureDevOps.Common;

namespace AnyStatus.Plugins.AzureDevOps.Releases
{
    public class ReleaseEnvironmentHealthCheck : ICheckHealth<ReleaseEnvironmentWidget>
    {
        public async Task Handle(HealthCheckRequest<ReleaseEnvironmentWidget> request, CancellationToken cancellationToken)
        {
            var widget = request.DataContext;

            if (!(widget.Parent is ReleaseWidget parent) || parent.DefinitionId == 0)
            {
                return;
            }

            var restClient = new RestClient(parent.ConnectionSettings.URL)
            {
                Authenticator = new HttpBasicAuthenticator("", parent.ConnectionSettings.PersonalAccessToken)
            };

            var deploymentsRequest = new RestRequest($"{parent.ConnectionSettings.Organization}/{parent.Project}/_apis/release/deployments");
            
            deploymentsRequest.AddParameter("definitionId", parent.DefinitionId);
            deploymentsRequest.AddParameter("definitionEnvironmentId", widget.DefinitionEnvironmentId);
            deploymentsRequest.AddParameter("$top", 10);
            deploymentsRequest.AddParameter("api-version", "5.0");

            var deploymentsResponse = await restClient.ExecuteTaskAsync<CollectionResponse<Deployment>>(deploymentsRequest, cancellationToken).ConfigureAwait(false);

            if (deploymentsResponse.IsSuccessful && deploymentsResponse.Data.Value.Any())
            {
                var deployments = deploymentsResponse.Data.Value.ToList();

                if (deployments.Any())
                {
                    deployments.Reverse();

                    var maxDuration = deployments.Max(b => b.Duration);

                    deployments.ForEach(deployment => deployment.Percentage = (double)deployment.Duration.Ticks / maxDuration.Ticks);
                }

                widget.JobHistory = deployments;
            }
        }
    }
}
