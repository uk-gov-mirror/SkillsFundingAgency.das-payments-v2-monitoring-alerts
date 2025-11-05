using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace SFA.DAS.Payments.Monitoring.Alerts.Function.Services
{
    public interface ISlackService
    {
        Task PostSlackAlert(string appInsightsAlertPayload, string slackChannelUri);
    }
}