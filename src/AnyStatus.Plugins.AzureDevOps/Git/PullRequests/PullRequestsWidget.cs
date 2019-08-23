using AnyStatus.API;
using AnyStatus.Plugins.AzureDevOps.API;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Xml.Serialization;

namespace AnyStatus.Plugins.AzureDevOps.Git.PullRequests
{
    [DisplayColumn("Azure DevOps")]
    [DisplayName("Pull Requests")]
    [XmlType(TypeName = "AzureDevOpsPullRequests_v1")]
    [Description("Retrieve all pull requests matching a specified criteria.")]
    public class PullRequestsWidget : Metric, ISchedulable, IInitializable//, IWebPage
    {
        public PullRequestsWidget()
        {
            ShowNotifications = false;
        }

        [Required]
        public AzureDevOpsConnectionSettings ConnectionSettings { get; set; } = new AzureDevOpsConnectionSettings();


        [XmlIgnore]
        [Browsable(false)]
        public bool IsInitialized { get; set; }

        [XmlIgnore]
        [Browsable(false)]
        public string RepositoryId { get; set; }

        [Required]
        [Category("Azure DevOps")]
        [Description("Required. The Azure DevOps GIT repository name.")]
        public string Repository { get; set; }

        [Required]
        [Category("Azure DevOps")]
        [Description("Required. The Azure DevOps project name.")]
        public string Project { get; set; }

        [Category("Azure DevOps")]
        [DisplayName("Target Branch")]
        [Description("Optional. Search for pull requests into this branch. Example: refs/heads/master")]
        public string TargetBranch { get; set; }

        [Category("Azure DevOps")]
        [DisplayName("Source Branch")]
        [Description("Optional. Search for pull requests from this branch. Example: refs/heads/master")]
        public string SourceBranch { get; set; }
    }
}
