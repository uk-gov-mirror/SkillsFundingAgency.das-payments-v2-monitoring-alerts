using System.Threading.Tasks;

namespace SFA.DAS.Payments.Monitoring.Alerts.Function.Services
{
    public interface ITeamsService
    {
        Task PostTeamsAlert(string appInsightsAlertPayload, string teamsWebhookURL);
    }
}