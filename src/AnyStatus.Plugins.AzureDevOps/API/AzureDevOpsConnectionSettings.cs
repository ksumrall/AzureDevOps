using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using AnyStatus.API;
using Xceed.Wpf.Toolkit.PropertyGrid.Attributes;

namespace AnyStatus.Plugins.AzureDevOps.API
{
    [ExpandableObject]
    [Category("Azure DevOps")]
    [DisplayName(" Connection Settings")]
    [Description("Required. The Azure DevOps connection settings.")]
    public class AzureDevOpsConnectionSettings
    {
        [Required]
        [DisplayName("Name")]
        [Description("Required. The connection display name.")]
        public string Name { get; set; } = "Azure DevOps";

        [Required]
        [DisplayName("URL")]
        [Description("Required. Azure DevOps API URL address.")]
        public string URL { get; set; } = "https://dev.azure.com";

        [Required]
        [Description("Required. Azure DevOps organization name.")]
        public string Organization { get; set; }

        [Required]
        [DisplayName("Personal Access Token")]
        [Description("Required. Azure DevOps personal access token.")]
        [Editor(typeof(PasswordEditor), typeof(PasswordEditor))]
        public string PersonalAccessToken { get; set; }

        public override string ToString()
        {
            return Name;
        }
    }
}
