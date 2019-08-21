using AnyStatus.API;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Xml.Serialization;
using AnyStatus.Plugins.AzureDevOps.API;

namespace AnyStatus.Plugins.AzureDevOps.WorkItems
{
    [DisplayName("Work Items")]
    [DisplayColumn("Azure DevOps")]
    [XmlType(TypeName = "AzureDevOpsWorkItems_v2")]
    [Description("Get work items from Azure DevOps.")]
    public class WorkItemsWidget : Metric, ISchedulable
    {
        public WorkItemsWidget()
        {
            ShowNotifications = false;
            ShowErrorNotifications = false;
        }

        [Required]
        public AzureDevOpsConnectionSettings ConnectionSettings { get; set; } = new AzureDevOpsConnectionSettings();

        [Required]
        [Category("Azure DevOps")]
        [DisplayName("Assigned To")]
        [Description("Required. The assignee name or macro. Use @Me macro to get work item assigned to you.")]
        public string AssignedTo { get; set; } = "@Me";
    }
}
