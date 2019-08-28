using AnyStatus.API;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Xml.Serialization;

namespace AnyStatus.Plugins.AzureDevOps.Git.PullRequests
{
    [Browsable(false)]
    [DisplayName("Pull Request")]
    [DisplayColumn("Azure DevOps")]
    public class PullRequestWidget : Widget
    {
        [XmlIgnore]
        [Browsable(false)]
        public string PullRequestId { get; set; }
    }
}
