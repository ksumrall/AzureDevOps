using AnyStatus.API;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace AnyStatus.Plugins.AzureDevOps.Builds
{
    public class BuildHealthCheck : ICheckHealth<BuildWidget>
    {
        public async Task Handle(HealthCheckRequest<BuildWidget> request, CancellationToken cancellationToken)
        {
            var widget = request.DataContext;

            var api = new AzureDevOpsApi(widget.ConnectionSettings);

            var results = await api.GetBuildsAsync(widget.Project, widget.DefinitionId, 10, cancellationToken).ConfigureAwait(false);

            if (results.Count == 0)
            {
                widget.State = State.None;

                return;
            }

            var builds = results.Value.ToList();

            UpdateInfo(widget, builds[0]);

            UpdateStats(widget, builds);

            builds.Reverse();

            UpdateHistory(widget, builds);
        }

        private static void UpdateInfo(BuildWidget widget, Build lastBuild)
        {
            widget.State = lastBuild.State;
            widget.LastBuildId = lastBuild.Id;
            widget.Message = lastBuild.Text;
            widget.URL = lastBuild.Links["web"]["href"];
        }

        private static void UpdateStats(BuildWidget widget, IReadOnlyList<Build> builds)
        {
            if (widget.State != State.Running)
            {
                widget.ShowProgress = false;
                widget.Progress = 0;
                return;
            }

            var avgDurationTicks = builds.Where(b => b.Result == "succeeded").Average(b => b.Duration.Ticks);

            var durationTicks = builds[0].Duration.Ticks;

            widget.ShowProgress = true;

            widget.Progress = (int)(durationTicks / avgDurationTicks * 100);

            var remainingTicks = avgDurationTicks - durationTicks;

            if (remainingTicks > DateTime.MinValue.Ticks)
            {
                widget.Message += $"\nRemaining: {new DateTime((long)remainingTicks):HH:mm:ss}";
            }
            else
            {
                widget.Message += $"\nBuild is taking {new DateTime((long)remainingTicks * -1):HH:mm:ss} longer than expected";
            }
        }

        private static void UpdateHistory(IJobHistory widget, ICollection<Build> builds)
        {
            var maxDurationTicks = builds.Max(b => b.Duration).Ticks;

            foreach (var b in builds)
            {
                b.Percentage = (double)b.Duration.Ticks / maxDurationTicks;
            }

            widget.JobHistory = builds;
        }
    }
}
