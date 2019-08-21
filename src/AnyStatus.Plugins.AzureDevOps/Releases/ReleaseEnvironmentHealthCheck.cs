using AnyStatus.API;
using AnyStatus.Plugins.AzureDevOps.API;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace AnyStatus.Plugins.AzureDevOps.Releases
{
    public class ReleaseEnvironmentHealthCheck : ICheckHealth<ReleaseEnvironmentWidget>
    {
        public async Task Handle(HealthCheckRequest<ReleaseEnvironmentWidget> request, CancellationToken cancellationToken)
        {
            var widget = request.DataContext;

            if (widget.Parent is ReleaseWidget parent && parent.DefinitionId != 0)
            {
                var api = new AzureDevOpsApi(parent.ConnectionSettings);

                var response = await api.GetDeploymentsAsync(parent.Project, parent.DefinitionId, widget.DefinitionEnvironmentId, 10, cancellationToken).ConfigureAwait(false);

                var deployments = response.Value.ToList();

                if (deployments.Any())
                {
                    deployments.Reverse();

                    var maxDuration = deployments.Max(deployment => deployment.Duration);

                    deployments.ForEach(deployment => deployment.Percentage = (double)deployment.Duration.Ticks / maxDuration.Ticks);
                }

                widget.JobHistory = deployments;
            }
        }
    }
}
