using System.Collections.Generic;

namespace AnyStatus.Plugins.AzureDevOps.API.Contracts
{
    public class WorkItemQueryResult
    {
        public IEnumerable<WorkItemReference> WorkItems { get; set; }
    }
}