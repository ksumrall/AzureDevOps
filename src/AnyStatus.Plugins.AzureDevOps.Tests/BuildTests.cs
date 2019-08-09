using AnyStatus.API;
using AnyStatus.Plugins.AzureDevOps.Builds;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Threading;
using System.Threading.Tasks;

namespace AnyStatus.Plugins.AzureDevOps.Tests
{
    [TestClass]
    public class BuildTests
    {
        [TestMethod]
        public async Task BuildHealthCheckTest()
        {
            var widget = new BuildWidget
            {
                Project = "AnyStatus",
                Definition = "Master",
                ConnectionSettings = new AzureDevOpsConnectionSettings
                {
                    Organization = "production",
                    PersonalAccessToken = "mge4tex7jlpf3xtwg4lppmju5l2eenwebdv4m22mkmiotkdtx2za"
                }
            };

            await new InitializeBuild().Handle(InitializeRequest.Create(widget), CancellationToken.None).ConfigureAwait(false);

            await new BuildHealthCheck().Handle(HealthCheckRequest.Create(widget), CancellationToken.None).ConfigureAwait(false);
        }
    }
}
