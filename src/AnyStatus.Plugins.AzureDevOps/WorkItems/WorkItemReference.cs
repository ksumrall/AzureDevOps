using System.Collections.Generic;

namespace AnyStatus.Plugins.AzureDevOps.WorkItems
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
