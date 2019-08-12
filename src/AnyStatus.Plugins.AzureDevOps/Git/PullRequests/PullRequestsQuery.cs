using AnyStatus.API;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace AnyStatus.Plugins.AzureDevOps.Git.PullRequests
{
    public class PullRequestsQuery : IMetricQuery<PullRequestsWidget>
    {
        public Task Handle(MetricQueryRequest<PullRequestsWidget> request, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
    }
}
