using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using SFA.DAS.Payments.Monitoring.Alerts.Function.Services;

namespace SFA.DAS.Payments.Monitoring.Alerts.Function
{
    public class SendTeamsAlert
    {
        private readonly ITeamsService _teamsService;

        public SendTeamsAlert(ITeamsService teamsService)
        {
            _teamsService = teamsService;
        }

        [FunctionName("HttpTrigger1")]
        public async Task<IActionResult> SendToChannelOne(
            [HttpTrigger(AuthorizationLevel.Function, "get", "post", Route = null)] HttpRequest req, ILogger log)
        {
            var teamsWebhookURL =
                Environment.GetEnvironmentVariable("TeamsWebhookURL", EnvironmentVariableTarget.Process);

            log.LogInformation("HttpTrigger1 function processed a request.");

            var requestBody = await new StreamReader(req.Body).ReadToEndAsync();

            log.LogInformation($"Request: {requestBody}.");

            await _teamsService.PostTeamsAlert(requestBody, teamsWebhookURL);

            return new OkObjectResult("");
        }

        [FunctionName("HttpTrigger2")]
        public async Task<IActionResult> SendToChannelTwo(
            [HttpTrigger(AuthorizationLevel.Function, "get", "post", Route = null)] HttpRequest req, ILogger log)
        {
            var teamsWebhookURL =
                Environment.GetEnvironmentVariable("TeamsWebhookURL2", EnvironmentVariableTarget.Process);

            log.LogInformation("HttpTrigger2 function processed a request.");

            var requestBody = await new StreamReader(req.Body).ReadToEndAsync();

            log.LogInformation($"Request: {requestBody}.");

            await _teamsService.PostTeamsAlert(requestBody, teamsWebhookURL);

            return new OkObjectResult("");
        }
    }
}