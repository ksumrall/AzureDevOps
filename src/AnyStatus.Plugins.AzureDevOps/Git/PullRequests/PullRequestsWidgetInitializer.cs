using AnyStatus.API;
using AnyStatus.Plugins.AzureDevOps.API;
using System.Threading;
using System.Threading.Tasks;

namespace AnyStatus.Plugins.AzureDevOps.Git.PullRequests
{
    public class PullRequestsWidgetInitializer : IInitialize<PullRequestsWidget>
    {
        public async Task Handle(InitializeRequest<PullRequestsWidget> request, CancellationToken cancellationToken)
        {
            var api = new AzureDevOpsApi(request.DataContext.ConnectionSettings);

            var repository = await api.GetRepositoryAsync(request.DataContext.Project, request.DataContext.Repository, cancellationToken).ConfigureAwait(false);

            if (string.IsNullOrEmpty(repository.Id))
            {
                request.DataContext.State = State.Unknown;
            }
            else
            {
                request.DataContext.RepositoryId = repository.Id;
            }
        }
    }
}
