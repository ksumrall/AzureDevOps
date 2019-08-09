using AnyStatus.API;

namespace AnyStatus.Plugins.AzureDevOps.Builds
{
    public class BuildWebPage : OpenWebPage<BuildWidget>
    {
        public BuildWebPage(IProcessStarter ps) : base(ps) { }
    }
}
