using AnyStatus.API;

namespace AnyStatus.Plugins.AzureDevOps.WorkItems
{

    public class WorkItemWebPage : OpenWebPage<WorkItemWidget>
    {
        public WorkItemWebPage(IProcessStarter ps) : base(ps) { }
    }
}
