using AnyStatus.API;
using AnyStatus.API.Common.Services;
using RestSharp;
using RestSharp.Authenticators;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using AnyStatus.Plugins.AzureDevOps.Common;

namespace AnyStatus.Plugins.AzureDevOps.WorkItems
{
    //https://docs.microsoft.com/en-us/rest/api/azure/devops/wit/work%20items/list?view=azure-devops-rest-5.0#workitem

    public class WorkItemsQuery : IMetricQuery<WorkItemsWidget>
    {
        private readonly IUiAction _uiAction;

        public WorkItemsQuery(IUiAction uiAction)
        {
            _uiAction = uiAction;
        }

        public async Task Handle(MetricQueryRequest<WorkItemsWidget> request, CancellationToken cancellationToken)
        {
            const string workItemsQuery = "SELECT [System.Id] FROM WorkItems " +
                                          "WHERE [System.AssignedTo] = {0} " +
                                          "AND [State] NOT IN ('Done','Closed','Inactive','Completed')";

            var restClient = new RestClient(request.DataContext.ConnectionSettings.URL)
            {
                Authenticator = new HttpBasicAuthenticator(string.Empty, request.DataContext.ConnectionSettings.PersonalAccessToken)
            };

            var workItemReferencesRequest = new RestRequest($"{request.DataContext.ConnectionSettings.Organization}/_apis/wit/wiql?api-version=5.0", Method.POST);

            workItemReferencesRequest.AddJsonBody(new { query = string.Format(workItemsQuery, request.DataContext.AssignedTo) });

            var workItemReferencesResponse = await restClient.ExecuteTaskAsync<WorkItemReferences>(workItemReferencesRequest, cancellationToken).ConfigureAwait(false);

            if (workItemReferencesResponse.ErrorException != null)
            {
                throw workItemReferencesResponse.ErrorException;
            }

            if (!workItemReferencesResponse.IsSuccessful)
            {
                throw new Exception(message: $"Azure DevOps API Response: {workItemReferencesResponse.StatusDescription} ({workItemReferencesResponse.StatusCode}).\n{workItemReferencesResponse.Content}");
            }

            var ids = workItemReferencesResponse.Data.WorkItems.Select(w => w.Id).ToList();

            if (ids.Count == 0)
            {
                var widget = request.DataContext;

                _uiAction.Invoke(() =>
                {
                    widget.Clear();
                    widget.Value = 0;
                    widget.State = State.Ok;
                });

                return;
            }

            var workItemsRequest = new RestRequest($"{request.DataContext.ConnectionSettings.Organization}/_apis/wit/workitemsbatch?api-version=5.0", Method.POST);

            workItemsRequest.AddJsonBody(new Dictionary<string, object>
            {
                ["$expand"] = "Links",
                ["fields"] = new[] { "System.Id", "System.Title", "System.WorkItemType", "System.TeamProject" },
                ["ids"] = ids,
            });

            var workItemsResponse = await restClient.ExecuteTaskAsync<CollectionResponse<WorkItem>>(workItemsRequest, cancellationToken).ConfigureAwait(false);

            if (workItemsResponse.ErrorException != null)
            {
                throw workItemsResponse.ErrorException;
            }

            _uiAction.Invoke(() => UpdateWidget(request.DataContext, workItemReferencesResponse.Data.WorkItems, workItemsResponse.Data));
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
