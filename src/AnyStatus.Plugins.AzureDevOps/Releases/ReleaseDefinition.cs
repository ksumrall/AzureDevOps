using System.Collections.Generic;

namespace AnyStatus.Plugins.AzureDevOps.Releases
{
    internal class ReleaseDefinition
    {
        public int Id { get; set; }

        public Dictionary<string, Dictionary<string, string>> Links { get; set; }
    }
}