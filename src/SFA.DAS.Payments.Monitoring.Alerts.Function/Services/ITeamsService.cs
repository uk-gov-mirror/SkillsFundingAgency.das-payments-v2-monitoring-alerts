using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace SFA.DAS.Payments.Monitoring.Alerts.Function.Services
{
    public interface ITeamsService
    {
        Task<dynamic> PostTeamsAlert(string appInsightsAlertPayload, string teamsWebhookURL, ILogger log);
    }
}