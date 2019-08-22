using AnyStatus.API;
using AnyStatus.API.Common.Services;
using AnyStatus.Plugins.AzureDevOps.API;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace AnyStatus.Plugins.AzureDevOps.WorkItems.Query
{
    public class WorkItemQueryWidgetInitializer : IInitialize<WorkItemQueryWidget>
    {
        private readonly ILogger _logger;
        private readonly IUiAction _uiAction;

        public WorkItemQueryWidgetInitializer(ILogger logger, IUiAction uiAction)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _uiAction = uiAction ?? throw new ArgumentNullException(nameof(uiAction)); ;
        }

        public async Task Handle(InitializeRequest<WorkItemQueryWidget> request, CancellationToken cancellationToken)
        {
            var api = new AzureDevOpsApi(request.DataContext.ConnectionSettings);

            var workItemQueries =
                await api.GetWorkItemQueriesAsync(request.DataContext.Project, request.DataContext.Query, 1,
                    cancellationToken).ConfigureAwait(false);

            if (workItemQueries.Count == 0)
            {
                request.DataContext.State = State.Unknown;
                request.DataContext.QueryId = string.Empty;
                request.DataContext.URL = string.Empty;

                _uiAction.Invoke(request.DataContext.Clear);

                request.DataContext.Clear();

                _logger.Error($"Work item query {request.DataContext.Query} was not found.");
            }
            else
            {
                var query = workItemQueries.Value.First();
                request.DataContext.QueryId = query.Id;
                request.DataContext.URL = query.Links["html"]["href"];
            }
        }
    }
}
