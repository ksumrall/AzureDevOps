using AnyStatus.API;
using AnyStatus.API.Common.Services;
using AnyStatus.Plugins.AzureDevOps.API;
using AnyStatus.Plugins.AzureDevOps.API.Contracts;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace AnyStatus.Plugins.AzureDevOps.Git.PullRequests
{
    public class PullRequestsQuery : IMetricQuery<PullRequestsWidget>
    {
        private readonly IUiAction _uiAction;

        public PullRequestsQuery(IUiAction uiAction)
        {
            _uiAction = uiAction ?? throw new ArgumentNullException(nameof(uiAction));
        }

        public async Task Handle(MetricQueryRequest<PullRequestsWidget> request, CancellationToken cancellationToken)
        {
            if (string.IsNullOrEmpty(request.DataContext.RepositoryId))
            {
                return;
            }

            var api = new AzureDevOpsApi(request.DataContext.ConnectionSettings);

            var pullRequests = await api.GetPullRequestsAsync(request.DataContext.Project, request.DataContext.RepositoryId, cancellationToken).ConfigureAwait(false);

            request.DataContext.Value = pullRequests.Count;

            var synchronizer = GetSynchronizer(request);

            _uiAction.Invoke(() => synchronizer.Synchronize(pullRequests.Value.ToList(), request.DataContext.Items));

            request.DataContext.State = State.Ok;
        }

        private static CollectionSynchronizer<GitPullRequest, Item> GetSynchronizer(MetricQueryRequest<PullRequestsWidget> request)
        {
            return new CollectionSynchronizer<GitPullRequest, Item>
            {
                Compare = (pr, item) => item is PullRequestWidget widget && pr.PullRequestId == widget.PullRequestId,
                Remove = item => request.DataContext.Remove(item),
                Update = (issue, item) => item.Name = issue.Title,
                Add = issue => request.DataContext.Add(new PullRequestWidget
                {
                    Name = issue.Title,
                    PullRequestId = issue.PullRequestId,
                    State = State.Ok,
                    Interval = 0
                })
            };
        }
    }
}
