using System.Threading;
using System.Threading.Tasks;
using AnyStatus.API;

namespace AnyStatus.Plugins.AzureDevOps.Git.PullRequests
{
    public class PullRequestsWidgetInitializer : IInitialize<PullRequestsWidget>
    {
        public Task Handle(InitializeRequest<PullRequestsWidget> request, CancellationToken cancellationToken)
        {
            throw new System.NotImplementedException();
        }
    }
}
