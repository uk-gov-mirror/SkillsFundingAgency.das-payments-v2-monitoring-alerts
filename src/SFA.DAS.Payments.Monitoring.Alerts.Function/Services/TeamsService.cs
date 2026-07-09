using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using SFA.DAS.Payments.Monitoring.Alerts.Function.Helpers;
using SFA.DAS.Payments.Monitoring.Alerts.Function.JsonHelpers;
using SFA.DAS.Payments.Monitoring.Alerts.Function.Models;
using SFA.DAS.Payments.Monitoring.Alerts.Function.Models.TeamsPayload;
using SFA.DAS.Payments.Monitoring.Alerts.Function.TypedClients;

namespace SFA.DAS.Payments.Monitoring.Alerts.Function.Services
{
    public class TeamsService : ITeamsService
    {
        private readonly IAppInsightsClient _appInsightsClient;
        private readonly ITeamsAlertHelper _teamsAlertHelper;
        private readonly ITeamsClient _teamsClient;
        private readonly IDynamicJsonDeserializer _deserializer;

        public TeamsService(IDynamicJsonDeserializer deserializer,
                            ITeamsAlertHelper teamsAlertHelper,
                            ITeamsClient teamsClient,
                            IAppInsightsClient appInsightsClient)
        {
            _deserializer = deserializer;
            _teamsAlertHelper = teamsAlertHelper;
            _appInsightsClient = appInsightsClient;
            _teamsClient = teamsClient;
        }

        public async Task<dynamic> PostTeamsAlert(string appInsightsAlertPayload, string teamsWebhookURL, ILogger log)
        {
            try
            {
                dynamic alert = _deserializer.Deserialize(appInsightsAlertPayload);

                string searchResultApiUrl = alert.data.alertContext.condition.allOf[0].linkToSearchResultsAPI;
                alert.data.alertContext.SearchResults =
                    await _appInsightsClient.GetSearchResultsAsync(searchResultApiUrl);

                var severity = alert.data.essentials.severity;
                string alertEmoji = _teamsAlertHelper.GetEmoji(severity);
                string alertColour = _teamsAlertHelper.GetBackgroundColour(severity);

                var appInsightsSearchResultsUiLink = alert.data.alertContext.condition.allOf[0].linkToSearchResultsUI;

                foreach (var table in alert.data.alertContext.SearchResults.tables)
                {
                    foreach (var row in table.rows)
                    {
                        var customDimensions = JsonSerializer.Deserialize<Dictionary<string, string>>(row[3]);
                        var customMeasurements = JsonSerializer.Deserialize<Dictionary<string, double>>(row[4]);
                        DateTime timestamp = DateTime.Parse(row[0]);
                        Dictionary<string, string> alertVariables =
                            _teamsAlertHelper.ExtractAlertVariables(customMeasurements, customDimensions, timestamp);
                        string alertDescription = alert.data.essentials.description;
                        await PostTeamsAlert(alertVariables, teamsWebhookURL, alertDescription, alertEmoji, alertColour,
                            appInsightsSearchResultsUiLink, timestamp);
                    }
                }

                return null;
            }
            catch (Exception ex)
            {
                log.LogError("Error while processing app insight payload - " + ex.Message);
                return new
                {
                    exception = ex.Message,
                    innerException = ex.InnerException
                };
            }
        }

        private async Task PostTeamsAlert(Dictionary<string, string> alertVariables,
                                          string teamsWebhookURL,
                                          string alertDescription,
                                          string alertEmoji,
                                          string alertColour,
                                          string appInsightsSearchResultsUiLink,
                                          DateTime timestamp)
        {

            var alertParameters = new AlertParameters
            {
                AlertEmoji = alertEmoji,
                AlertColour = alertColour,
                Timestamp = timestamp,
                JobId = alertVariables["JobId"],
                AcademicYear = alertVariables["AcademicYear"],
                CollectionPeriod = alertVariables["CollectionPeriod"],
                CollectionPeriodPayments = alertVariables["CollectionPeriodPayments"],
                YearToDatePayments = alertVariables["YearToDatePayments"],
                NumberOfLearners = alertVariables["NumberOfLearners"],
                AccountedForPayments = alertVariables["AccountedForPayments"],
                AlertTitle = _teamsAlertHelper.GetAlertTitle(alertDescription, alertVariables),
                AppInsightsSearchResultsUiLink = appInsightsSearchResultsUiLink
            };
            var teamsPayload = new
            {
                attachments = new List<object>(){
                    new
                    {
                        contentType = "application/vnd.microsoft.card.adaptive",
                        content = new
                        {
                            schema = "https://adaptivecards.io/schemas/adaptive-card.json",
                            type = "AdaptiveCard",
                            version = "1.5",
                            body = new List<TeamsCardContainer>()
                            {
                                _teamsAlertHelper.BuildAlertPayload(alertParameters)
                            },
                            actions = new List<object>()
                            {
                                new{
                                    type= "Action.OpenUrl",
                                    title= "View in Azure App Insights",
                                    url= alertParameters.AppInsightsSearchResultsUiLink
                                }
                            }
                        }
                    }
                }
            };

            await _teamsClient.PostAsJsonAsync(teamsWebhookURL, teamsPayload);
        }
    }
}