using AnyStatus.API;
using AnyStatus.API.Common.Services;
using AnyStatus.Plugins.AzureDevOps.API;
using AnyStatus.Plugins.AzureDevOps.API.Contracts;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace AnyStatus.Plugins.AzureDevOps.WorkItems.Query
{
    public class WorkItemQueryById : IMetricQuery<WorkItemQueryWidget>
    {
        private readonly IUiAction _uiAction;

        public WorkItemQueryById(IUiAction uiAction)
        {
            _uiAction = uiAction;
        }

        public async Task Handle(MetricQueryRequest<WorkItemQueryWidget> request, CancellationToken cancellationToken)
        {
            if (string.IsNullOrEmpty(request.DataContext.QueryId))
            {
                return;
            }

            var api = new AzureDevOpsApi(request.DataContext.ConnectionSettings);

            var workItemQueryResult = await api.QueryWorkItemsByIdAsync(request.DataContext.QueryId, cancellationToken).ConfigureAwait(false);

            var ids = workItemQueryResult.WorkItems.Select(w => w.Id).ToList();

            if (ids.Any())
            {
                var workItems = await api.GetWorkItemsAsync(ids, cancellationToken).ConfigureAwait(false);

                _uiAction.Invoke(() => UpdateWidget(request.DataContext, workItemQueryResult.WorkItems, workItems));
            }
            else
            {
                request.DataContext.Value = 0;
                request.DataContext.State = State.Ok;

                _uiAction.Invoke(request.DataContext.Clear);
            }
        }

        private static void UpdateWidget(Metric metric, IEnumerable<WorkItemReference> references, CollectionResponse<WorkItem> workItemsResponse)
        {
            metric.Value = workItemsResponse.Count;

            foreach (var removedWorkItem in metric.Items.OfType<WorkItemWidget>().Where(w => references.All(r => r.Id != w.WorkItemId)).ToList())
            {
                metric.Remove(removedWorkItem);
            }

            foreach (var workItem in workItemsResponse.Value)
            {
                AddOrUpdateWorkItem(metric, workItem);
            }

            metric.State = State.Ok;
        }

        private static void AddOrUpdateWorkItem(Widget widget, WorkItem workItem)
        {
            var workItemWidget = (WorkItemWidget)widget.Items.FirstOrDefault(i => i is WorkItemWidget w && w.WorkItemId == workItem.Id);

            if (workItemWidget == null)
            {
                workItemWidget = new WorkItemWidget();

                widget.Add(workItemWidget);
            }

            workItemWidget.Name = workItem.Fields["System.Title"];
            workItemWidget.WorkItemId = workItem.Fields["System.Id"];
            workItemWidget.URL = workItem.Links["html"]["href"];

            workItemWidget.Message = new StringBuilder()
                .AppendLine($"Title: {workItem.Fields["System.Title"]}")
                .AppendLine($"Type: {workItem.Fields["System.WorkItemType"]}")
                .Append($"Project: {workItem.Fields["System.TeamProject"]}")
                .ToString();

            workItemWidget.State = State.Ok;
        }
    }
}
