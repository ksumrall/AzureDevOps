using AnyStatus.API;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Xml.Serialization;

namespace AnyStatus.Plugins.AzureDevOps.WorkItems
{
    [Browsable(false)]
    [DisplayName("Work Item")]
    [DisplayColumn("Azure DevOps")]
    [XmlType(TypeName = "AzureDevOpsWorkItem_v2")]
    [Description("Azure DevOps work item.")]
    public class WorkItemWidget : Widget, IWebPage
    {
        public WorkItemWidget()
        {
            ShowNotifications = false;
            ShowErrorNotifications = false;
        }

        [ReadOnly(true)]
        [DisplayName("Work Item Id")]
        public string WorkItemId { get; set; }

        [XmlIgnore]
        [Browsable(false)]
        public string URL { get; set; }
    }
}
