using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Threading.Tasks;
using SFA.DAS.Payments.Monitoring.Alerts.Function.Helpers;
using SFA.DAS.Payments.Monitoring.Alerts.Function.JsonHelpers;
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

        public async Task PostTeamsAlert(string appInsightsAlertPayload, string teamsWebhookURL)
        {
            dynamic alert = _deserializer.Deserialize(appInsightsAlertPayload);

            string searchResultApiUrl = alert.data.alertContext.condition.allOf[0].linkToSearchResultsAPI;
            alert.data.alertContext.SearchResults = await _appInsightsClient.GetSearchResultsAsync(searchResultApiUrl);

            var severity = alert.data.essentials.severity;
            string alertEmoji = _teamsAlertHelper.GetEmoji(severity);

            var appInsightsSearchResultsUiLink = alert.data.alertContext.condition.allOf[0].linkToSearchResultsUI;

            foreach (var table in alert.data.alertContext.SearchResults.tables)
            {
                foreach (var row in table.rows)
                {
                    var customDimensions = JsonSerializer.Deserialize<Dictionary<string, string>>(row[3]);
                    var customMeasurements = JsonSerializer.Deserialize<Dictionary<string, double>>(row[4]);
                    DateTime timestamp = DateTime.Parse(row[0]);
                    Dictionary<string,string> alertVariables = _teamsAlertHelper.ExtractAlertVariables(customMeasurements, customDimensions, timestamp);
                    string alertDescription = alert.data.essentials.description;
                    await PostTeamsAlert(alertVariables, teamsWebhookURL, alertDescription, alertEmoji, appInsightsSearchResultsUiLink, timestamp);
                }
            }
        }

        private async Task PostTeamsAlert(Dictionary<string, string> alertVariables,
                                          string teamsWebhookURL,
                                          string alertDescription,
                                          string alertEmoji,
                                          string appInsightsSearchResultsUiLink,
                                          DateTime timestamp)
        {
            string alertTitle = _teamsAlertHelper.GetAlertTitle(alertDescription, alertVariables);
            
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
                            body = new List<object>()
                            {
                                _teamsAlertHelper.BuildAlertPayload(alertEmoji,
                                    timestamp,
                                    alertVariables["JobId"],
                                    alertVariables["AcademicYear"],
                                    alertVariables["CollectionPeriod"],
                                    alertTitle,
                                    appInsightsSearchResultsUiLink)
                            }
                        }
                    }
                }
            };

            await _teamsClient.PostAsJsonAsync(teamsWebhookURL, teamsPayload);
        }
    }
}