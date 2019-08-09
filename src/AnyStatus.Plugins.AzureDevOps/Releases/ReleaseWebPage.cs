using AnyStatus.API;

namespace AnyStatus.Plugins.AzureDevOps.Releases
{
    public class ReleaseWebPage : OpenWebPage<ReleaseWidget>
    {
        public ReleaseWebPage(IProcessStarter ps) : base(ps) { }
    }
}
