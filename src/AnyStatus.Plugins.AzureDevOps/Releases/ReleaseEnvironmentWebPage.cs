using AnyStatus.API;
using System;

namespace AnyStatus.Plugins.AzureDevOps.Releases
{
    public class ReleaseEnvironmentWebPage : RequestHandler<OpenWebPageRequest<ReleaseEnvironmentWidget>>
    {
        private readonly IProcessStarter _ps;

        public ReleaseEnvironmentWebPage(IProcessStarter ps)
        {
            _ps = ps ?? throw new ArgumentNullException(nameof(ps));
        }

        protected override void HandleCore(OpenWebPageRequest<ReleaseEnvironmentWidget> request)
        {
            if (request.DataContext.Parent is ReleaseWidget parent)
            {
                var url = $"https://dev.azure.com/{Uri.EscapeDataString(parent.ConnectionSettings.Organization)}/{Uri.EscapeDataString(parent.Project)}/_releaseProgress?_a=release-environment-logs&releaseId={parent.LastReleaseId}&environmentId={request.DataContext.DeploymentId}";

                _ps.Start(url);
            }
            else
            {
                throw new InvalidOperationException("Parent was not found.");
            }
        }
    }
}
