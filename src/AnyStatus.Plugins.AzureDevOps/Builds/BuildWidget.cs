using AnyStatus.API;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Xml.Serialization;
using AnyStatus.Plugins.AzureDevOps.API;

namespace AnyStatus.Plugins.AzureDevOps.Builds
{
    [DisplayName("Build")]
    [DisplayColumn("Azure DevOps")]
    [XmlType(TypeName = "AzureDevOpsBuild_v1")]
    [Description("Get build from Azure DevOps.")]
    public class BuildWidget : Widget,
        IHealthCheck, ISchedulable, IWebPage, IInitializable,
        IJobHistory, IReportProgress, IStartable, IStoppable
    {
        private int _progress;
        private bool _showProgress;
        private IEnumerable<JobRun> _jobHistory;

        public BuildWidget()
        {
            ShowErrorNotifications = false;
        }

        public AzureDevOpsConnectionSettings ConnectionSettings { get; set; } = new AzureDevOpsConnectionSettings();

        [Required]
        [Category("Azure DevOps")]
        [Description("Required. The Azure DevOps project name.")]
        public string Project { get; set; }

        [Required]
        [Category("Azure DevOps")]
        [DisplayName("Build Definition")]
        [Description("Required. The Azure DevOps build definition name.")]
        public string Definition { get; set; }

        [XmlIgnore]
        [ReadOnly(true)]
        [Browsable(false)]
        [Category("Azure DevOps")]
        public int DefinitionId { get; set; }

        [XmlIgnore]
        [Browsable(false)]
        public int LastBuildId { get; set; }

        [XmlIgnore]
        [Browsable(false)]
        public string URL { get; set; }

        [XmlIgnore]
        [Browsable(false)]
        public bool IsInitialized { get; set; }

        [XmlIgnore]
        [Browsable(false)]
        public bool ShowProgress
        {
            get => _showProgress;
            set
            {
                _showProgress = value;
                OnPropertyChanged();
            }
        }

        [XmlIgnore]
        [Browsable(false)]
        public int Progress
        {
            get => _progress;
            set
            {
                _progress = value;
                OnPropertyChanged();
            }
        }

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
            var clone = base.Clone();

            ((BuildWidget)clone).DefinitionId = 0;

            return clone;
        }
    }
}
