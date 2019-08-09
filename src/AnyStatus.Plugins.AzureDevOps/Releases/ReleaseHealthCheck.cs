using AnyStatus.API;
using AnyStatus.API.Common.Services;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace AnyStatus.Plugins.AzureDevOps.Releases
{
    public class ReleaseHealthCheck : ICheckHealth<ReleaseWidget>
    {
        private readonly IUiAction _uiAction;

        public ReleaseHealthCheck(IUiAction uiAction)
        {
            _uiAction = uiAction;
        }

        public async Task Handle(HealthCheckRequest<ReleaseWidget> request, CancellationToken cancellationToken)
        {
            var widget = request.DataContext;

            var api = new AzureDevOpsApi(widget.ConnectionSettings);

            var releases = await api.GetReleasesAsync(widget.Project, widget.DefinitionId, 1, cancellationToken).ConfigureAwait(false);

            if (releases.Count == 0)
            {
                widget.State = State.None;

                return;
            }

            var release = releases.Value.First();

            UpdateInfo(widget, release);

            _uiAction.Invoke(() =>
                UpdateEnvironments(widget, release));

            var deployments = await api.GetDeploymentsAsync(widget.Project, widget.DefinitionId, 10, cancellationToken).ConfigureAwait(false);

            if (deployments.Count > 0)
            {
                UpdateHistory(widget, deployments.Value.ToList());
            }
        }

        private static void UpdateHistory(ReleaseWidget widget, List<Deployment> deployments)
        {
            deployments.Reverse();

            var maxDuration = deployments.Max(b => b.Duration);

            deployments.ForEach(b => { b.Percentage = (double)b.Duration.Ticks / maxDuration.Ticks; });

            widget.JobHistory = deployments;
        }

        private static void UpdateInfo(ReleaseWidget widget, Release lastRelease)
        {
            widget.Message = lastRelease.Text;
            widget.LastReleaseId = lastRelease.Id;
        }

        private static void UpdateEnvironments(ReleaseWidget widget, Release release)
        {
            var removedEnvironments = widget.Items.OfType<ReleaseEnvironmentWidget>()
                .Where(w => release.Environments.All(e => e.DefinitionEnvironmentId != w.DefinitionEnvironmentId)).ToList();

            foreach (var environment in removedEnvironments)
            {
                widget.Remove(environment);
            }

            foreach (var environment in release.Environments)
            {
                AddOrUpdateEnvironment(widget, environment);
            }
        }

        private static void AddOrUpdateEnvironment(ReleaseWidget widget, Environment environment)
        {
            var envWidget = widget.Items.OfType<ReleaseEnvironmentWidget>().FirstOrDefault(w => w.DefinitionEnvironmentId == environment.DefinitionEnvironmentId);

            if (envWidget != null)
            {
                envWidget.State = environment.State;
                envWidget.DeploymentId = environment.Id;
            }
            else
            {
                widget.Add(new ReleaseEnvironmentWidget
                {
                    Name = environment.Name,
                    State = environment.State,
                    DeploymentId = environment.Id,
                    ReleaseId = environment.ReleaseId,
                    DefinitionEnvironmentId = environment.DefinitionEnvironmentId,
                });
            }
        }
    }
}
