using AnyStatus.API;
using System.ComponentModel;
using System.Xml.Serialization;

namespace AnyStatus.Plugins.AzureDevOps.Git.PullRequests
{
    public class PullRequestWidget : Widget
    {
        [XmlIgnore]
        [Browsable(false)]
        public string PullRequestId { get; set; }
    }
}
