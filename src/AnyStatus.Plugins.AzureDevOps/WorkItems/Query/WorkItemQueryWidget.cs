using AnyStatus.API;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Xml.Serialization;

namespace AnyStatus.Plugins.AzureDevOps.WorkItems.Query
{
    [DisplayColumn("Azure DevOps")]
    [DisplayName("Work Items Query")]
    [XmlType(TypeName = "AzureDevOpsWorkItemQuery_v2")]
    [Description("List bugs, user stories, or other work items.")]
    public class WorkItemQueryWidget : Metric, ISchedulable, IInitializable, IWebPage
    {
        public WorkItemQueryWidget()
        {
            ShowNotifications = false;
            ShowErrorNotifications = false;
        }

        [Required]
        public AzureDevOpsConnectionSettings ConnectionSettings { get; set; } = new AzureDevOpsConnectionSettings();

        [XmlIgnore]
        [Browsable(false)]
        public string QueryId { get; set; }

        [XmlIgnore]
        [Browsable(false)]
        public string WIQL { get; set; }

        [XmlIgnore]
        [Browsable(false)]
        public string URL { get; set; }

        [XmlIgnore]
        [Browsable(false)]
        public bool IsInitialized { get; set; }

        [Required]
        [Category("Azure DevOps")]
        [Description("Required. The Azure DevOps project name.")]
        public string Project { get; set; }

        [Required]
        [Category("Azure DevOps")]
        [DisplayName("Query")]
        [Description("Required. The name of the work items query.")]
        public string Query { get; set; }


    }
}
