using AnyStatus.API;

namespace AnyStatus.Plugins.AzureDevOps.WorkItems.Query
{

    public class WorkItemQueryWebPage : OpenWebPage<WorkItemQueryWidget>
    {
        public WorkItemQueryWebPage(IProcessStarter ps) : base(ps) { }
    }
}
