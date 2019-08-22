using System.Collections.Generic;

namespace AnyStatus.Plugins.AzureDevOps.API.Contracts
{
    public class WorkItemReference
    {
        public string Id { get; set; }
    }

    public class WorkItemReferences
    {
        public IEnumerable<WorkItemReference> WorkItems { get; set; }
    }
}
