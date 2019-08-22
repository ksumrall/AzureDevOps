using System.Collections.Generic;

namespace AnyStatus.Plugins.AzureDevOps.API.Contracts
{
    public class WorkItemQueryReference
    {
        public string Id { get; set; }

        public string Name { get; set; }

        public Dictionary<string, Dictionary<string, string>> Links { get; set; }
    }
}
