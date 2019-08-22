using System;
using System.Collections.Generic;
using AnyStatus.API;

namespace AnyStatus.Plugins.AzureDevOps.API.Contracts
{
    internal class Build : JobRun
    {
        public string Description => BuildNumber;

        public int Id { get; set; }

        public string BuildNumber { get; set; }

        public string Status { get; set; }

        public string Result { get; set; }

        public State State
        {
            get
            {
                switch (Status)
                {
                    case "notStarted":
                        return State.Queued;

                    case "inProgress":
                        return State.Running;

                    default:
                        break;
                }

                switch (Result)
                {
                    case "notStarted":
                        return State.Running;

                    case "succeeded":
                        return State.Ok;

                    case "failed":
                        return State.Failed;

                    case "partiallySucceeded":
                        return State.PartiallySucceeded;

                    case "canceled":
                        return State.Canceled;

                    default:
                        return State.Unknown;
                }
            }
        }

        public DateTime StartTime { get; set; }

        public DateTime FinishTime { get; set; }

        public Dictionary<string, Dictionary<string, string>> Links { get; set; }

        public TimeSpan Duration
        {
            get
            {
                if (State == State.Queued)
                    return TimeSpan.MinValue;

                if (State == State.Running)
                    return DateTime.Now - StartTime;

                return FinishTime - StartTime;
            }
        }

        public double Percentage { get; set; }

        public string URL => Links["web"]["href"];

        public IdentityRef RequestedBy { get; set; }

        public string Reason { get; set; }

        public string SourceBranch { get; set; }

        public string Text => $"Build {BuildNumber}\nDuration {Duration:hh\\:mm\\:ss}\nRequested by {RequestedBy?.DisplayName}\nReason {Reason}\nBranch {SourceBranch}";
    }
}
