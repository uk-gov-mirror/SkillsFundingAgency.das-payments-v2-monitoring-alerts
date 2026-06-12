using System.Collections.Generic;

namespace SFA.DAS.Payments.Monitoring.Alerts.Function.Models.TeamsPayload
{
    public class TeamsCardContainer
    {
        public string Style { get; set;}

        public string Type { get; set; }
        public List<TeamsCardItem> Items { get; set; }
    }
}
