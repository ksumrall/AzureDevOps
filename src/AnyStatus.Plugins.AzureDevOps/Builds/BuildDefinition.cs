using System.Collections.Generic;

namespace AnyStatus.Plugins.AzureDevOps.Builds
{
    internal class BuildDefinition
    {
        public int Id { get; set; }

        public Dictionary<string, Dictionary<string, string>> Links { get; set; }
    }
}