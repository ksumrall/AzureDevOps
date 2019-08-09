using AnyStatus.API;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Xml.Serialization;

namespace AnyStatus.Plugins.AzureDevOps.Releases
{
    [Browsable(false)]
    [DisplayColumn("Azure DevOps")]
    [DisplayName("Release Environment")]
    [XmlType(TypeName = "AzureDevOpsReleaseEnvironment_v1")]
    [Description("Get release environment from Azure DevOps.")]
    public class ReleaseEnvironmentWidget : Widget, IWebPage, IStartable, IStoppable, IHealthCheck, ISchedulable, IJobHistory
    {
        private IEnumerable<JobRun> _jobHistory;

        [ReadOnly(true)]
        [DisplayName("Environment Id")]
        public int DefinitionEnvironmentId { get; set; }
        
        [ReadOnly(true)]
        [DisplayName("Release Id")]
        public int ReleaseId { get; set; }

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

        [XmlIgnore]
        [ReadOnly(true)]
        [DisplayName("Deployment Id")]
        public int DeploymentId { get; set; }

        public string URL { get; set; }
    }
}
