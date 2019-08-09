using AnyStatus.API;
using System.Collections.Generic;
using System.Linq;

namespace AnyStatus.Plugins.AzureDevOps.Releases
{
    internal class Environment
    {
        public int Id { get; set; }

        public int DefinitionEnvironmentId { get; set; }

        public int ReleaseId { get; set; }

        public string Name { get; set; }

        public IEnumerable<Approval> PreDeployApprovals { get; set; }

        public string Status { get; set; }

        public State State
        {
            get
            {
                switch (Status)
                {
                    case "notStarted":
                        return State.None;

                    case "inProgress":
                        return PreDeployApprovals != null
                               && PreDeployApprovals.Any(k => k.Status != "approved") ? State.None : State.Running;

                    case "succeeded":
                        return State.Ok;

                    case "canceled":
                        return State.Canceled;

                    case "rejected":
                        return State.Failed;

                    case "queued":
                        return State.Queued;

                    case "scheduled":
                        return State.None;

                    case "partiallySucceeded":
                        return State.PartiallySucceeded;

                    default:
                        return State.None;
                }
            }
        }
    }
}