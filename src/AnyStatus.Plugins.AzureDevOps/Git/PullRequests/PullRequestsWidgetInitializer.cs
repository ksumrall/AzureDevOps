using AnyStatus.API;
using System.Threading;
using System.Threading.Tasks;

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
