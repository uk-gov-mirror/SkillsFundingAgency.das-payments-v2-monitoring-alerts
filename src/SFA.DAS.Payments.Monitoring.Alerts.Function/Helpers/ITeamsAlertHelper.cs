using System;
using System.Collections.Generic;
using SFA.DAS.Payments.Monitoring.Alerts.Function.Models;

namespace SFA.DAS.Payments.Monitoring.Alerts.Function.Helpers
{
    public interface ITeamsAlertHelper
    {
        public string GetEmoji(string severity);

        public object BuildAlertPayload(AlertParameters alertParameters);

        public Dictionary<string, string> ExtractAlertVariables(dynamic customMeasurements, dynamic customDimensions, DateTime timestamp);

        public string GetAlertTitle(string alertTitleFormat, Dictionary<string, string> alertVariables);

        public string GetAlertText(string alertTextFormat, Dictionary<string, string> alertVariables);
    }
}
