using System;
using AnyStatus.API;

namespace AnyStatus.Plugins.AzureDevOps.API.Contracts
{
    internal class Deployment : JobRun
    {
        public string Description => Duration.ToString();

        public string DeploymentStatus { get; set; }

        public double Percentage { get; set; } = 100;

        public DateTime StartedOn { get; set; }

        public DateTime CompletedOn { get; set; }

        public TimeSpan Duration => CompletedOn - StartedOn;

        public Environment ReleaseEnvironment { get; set; }

        public State State
        {
            get
            {
                switch (DeploymentStatus)
                {
                    case "all":
                        return State.Ok;

                    case "failed":
                        return State.Failed;

                    case "inProgress":
                        return State.Running;

                    case "notDeployed":
                        return State.None;

                    case "partiallySucceeded":
                        return State.PartiallySucceeded;

                    case "succeeded":
                        return State.Ok;

                    case "undefined":
                        return State.Unknown;

                    default:
                        return State.None;
                }
            }
        }
    }
}
