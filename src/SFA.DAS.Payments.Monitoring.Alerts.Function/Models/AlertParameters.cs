using System;

namespace SFA.DAS.Payments.Monitoring.Alerts.Function.Models
{
    public class AlertParameters
    {
        public string AlertEmoji { get; set; }
        public DateTime Timestamp { get; set; }
        public string JobId { get; set; }
        public string AcademicYear { get; set; }
        public string CollectionPeriod { get; set; }
        public string CollectionPeriodPayments { get; set; }
        public string YearToDatePayments { get; set; }
        public string NumberOfLearners { get; set; }
        public string AccountedForPayments { get; set; }
        public object AlertTitle { get; set; }
        public string AppInsightsSearchResultsUiLink { get; set; }
    }
}