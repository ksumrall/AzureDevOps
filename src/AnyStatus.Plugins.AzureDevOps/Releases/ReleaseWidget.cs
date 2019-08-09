using AnyStatus.API;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Xml.Serialization;

namespace AnyStatus.Plugins.AzureDevOps.Releases
{
    [DisplayName("Release")]
    [DisplayColumn("Azure DevOps")]
    [XmlType(TypeName = "AzureDevOpsRelease_v2")]
    [Description("Get release from Azure DevOps.")]
    public class ReleaseWidget : Widget, IHealthCheck, ISchedulable, IWebPage, IInitializable, IStartable, IJobHistory
    {
        private IEnumerable<JobRun> _jobHistory;

        public ReleaseWidget() : base(true)
        {
            ShowErrorNotifications = false;

            ConnectionSettings = new AzureDevOpsConnectionSettings
            {
                URL = "https://vsrm.dev.azure.com"
            };
        }

        [Required]
        public AzureDevOpsConnectionSettings ConnectionSettings { get; set; }

        [Required]
        [Category("Azure DevOps")]
        [Description("Required. The Azure DevOps project name.")]
        public string Project { get; set; }

        [Required]
        [Category("Azure DevOps")]
        [DisplayName("Release Definition")]
        [Description("Required. The Azure DevOps release definition name.")]
        public string Definition { get; set; }

        [XmlIgnore]
        [Browsable(false)]
        public int DefinitionId { get; set; }

        [XmlIgnore]
        [Browsable(false)]
        public string URL { get; set; }

        [XmlIgnore]
        [Browsable(false)]
        public int LastReleaseId { get; set; }

        [XmlIgnore]
        [Browsable(false)]
        public bool IsInitialized { get; set; }

        [XmlIgnore]
        [Browsable(false)]
        public IEnumerable<JobRun> JobHistory
        {
            get => _jobHistory;
            set
            {
                _jobHistory = value;
                OnPropertyChanged();
            }
        }

        public override object Clone()
        {
            var clone = (ReleaseWidget)base.Clone();

            clone.DefinitionId = 0;

            return clone;
        }
    }
}
