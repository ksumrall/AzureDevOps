using AnyStatus.API;

namespace AnyStatus.Plugins.AzureDevOps.WorkItems.Query
{

    public class WorkItemWebPage : OpenWebPage<WorkItemWidget>
    {
        public WorkItemWebPage(IProcessStarter ps) : base(ps) { }
    }
}
