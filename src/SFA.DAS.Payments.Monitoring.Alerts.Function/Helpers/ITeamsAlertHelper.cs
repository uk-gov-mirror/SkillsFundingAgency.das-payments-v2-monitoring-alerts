using System;
using System.Collections.Generic;

namespace SFA.DAS.Payments.Monitoring.Alerts.Function.Helpers
{
    public interface ITeamsAlertHelper
    {
        public string GetEmoji(string severity);

        public List<object> BuildAlertPayload(string alertEmoji,
                                              DateTime timestamp,
                                              string jobId,
                                              string academicYear,
                                              string collectionPeriod,
                                              string alertTitle,
                                              string appInsightsSearchResultsUiLink);

        public Dictionary<string, string> ExtractAlertVariables(dynamic customMeasurements, dynamic customDimensions, DateTime timestamp);

        public string GetAlertTitle(string alertTitleFormat, Dictionary<string, string> alertVariables);

        public string GetAlertText(string alertTextFormat, Dictionary<string, string> alertVariables);
    }
}
