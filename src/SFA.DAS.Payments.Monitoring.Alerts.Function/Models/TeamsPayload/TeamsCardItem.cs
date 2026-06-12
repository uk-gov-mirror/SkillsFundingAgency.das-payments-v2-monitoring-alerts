using System.Collections.Generic;

namespace SFA.DAS.Payments.Monitoring.Alerts.Function.Models.TeamsPayload
{
    public class TeamsCardItem
    {
        public string Type { get; set; }
        public string Text { get; set; }
        public string Weight { get; set; }
        public string Size { get; set; }
        public bool? Wrap { get; set; }
        public List<TeamsFact> Facts { get; set; }
    }
}
