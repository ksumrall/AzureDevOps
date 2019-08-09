using System.Collections.Generic;

namespace AnyStatus.Plugins.AzureDevOps
{
    public class CollectionResponse<T>
    {
        public int Count { get; set; }

        public IEnumerable<T> Value { get; set; }
    }
}