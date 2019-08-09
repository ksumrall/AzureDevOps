using AnyStatus.API;
using AnyStatus.Plugins.AzureDevOps.WorkItems;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Threading;
using System.Threading.Tasks;

namespace AnyStatus.Plugins.AzureDevOps.Tests
{
    [TestClass]
    public class WorkItemsTests
    {
        [TestMethod]
        public async Task WorkItemsQueryTest()
        {
            var widget = new WorkItemsWidget
            {
                ConnectionSettings = new AzureDevOpsConnectionSettings
                {
                    Organization = "production",
                    PersonalAccessToken = "mge4tex7jlpf3xtwg4lppmju5l2eenwebdv4m22mkmiotkdtx2za"
                }
            };

            await new WorkItemsQuery(new UiAction()).Handle(MetricQueryRequest.Create(widget), CancellationToken.None).ConfigureAwait(false);
        }
    }
}
