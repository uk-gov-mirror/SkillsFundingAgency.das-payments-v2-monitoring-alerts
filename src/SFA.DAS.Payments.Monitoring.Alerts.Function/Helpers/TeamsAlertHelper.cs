using System;
using System.Collections.Generic;
using SFA.DAS.Payments.Monitoring.Alerts.Function.Models;
using SFA.DAS.Payments.Monitoring.Alerts.Function.Models.TeamsPayload;

namespace SFA.DAS.Payments.Monitoring.Alerts.Function.Helpers
{
    public class TeamsAlertHelper : ITeamsAlertHelper
    {
        public TeamsCardContainer BuildAlertPayload(AlertParameters alertParameters)
        {
            return new TeamsCardContainer
            {
                Type = "Container",
                Style = alertParameters.AlertColour,
                Items = new List<TeamsCardItem>
                {
                    new TeamsCardItem
                    {
                        Type = "TextBlock",
                        Text = $"{alertParameters.AlertEmoji} {alertParameters.AlertTitle}.",
                        Weight = "bolder",
                        Size = "medium",
                        Wrap = true
                    },
                    new TeamsCardItem
                    {
                        Type = "FactSet",
                        Facts = new List<TeamsFact>
                        {
                            new TeamsFact
                            {
                                Title = "Timestamp",
                                Value = alertParameters.Timestamp.ToString("f")
                            },
                            new TeamsFact
                            {
                                Title = "Job",
                                Value = alertParameters.JobId
                            },
                            new TeamsFact
                            {
                                Title = "Academic Year",
                                Value = alertParameters.AcademicYear
                            },
                            new TeamsFact
                            {
                                Title = "Collection Period",
                                Value = alertParameters.CollectionPeriod
                            }
                        }
                    },
                    AddOptionalBlockFields(alertParameters.CollectionPeriodPayments, alertParameters.YearToDatePayments,
                        alertParameters.NumberOfLearners, alertParameters.AccountedForPayments)
                }
            };
        }

        public string GetEmoji(string severity)
        {
            return severity switch
            {
                "Sev0" or "Sev1" => "🚨",
                "Sev2" => "⚠️",
                "Sev3" => "✅",
                _ => string.Empty,
            };
        }

        public string GetBackgroundColour(string severity)
        {
            return severity switch
            {
                "Sev0" or "Sev1" => "Attention", // Red
                "Sev2" => "Warning", // Yellow
                "Sev3" => "Good", // Green
                _ => "Default", // Default to white
            };
        }

        private static string RemoveInvalidCharacters(string text)
        {
            return text.Replace("\"", "");
        }

        private static TeamsCardItem AddOptionalBlockFields(string collectionPeriodPayments, string yearToDatePayments, string numberOfLearners, string accountedForPayments)
        {
            var optionalFields = new List<TeamsFact>();

            if (!string.IsNullOrWhiteSpace(yearToDatePayments))
            {
                var yearTodatePaymentsText = string.Empty;
                try
                {
                    var yearToDatePaymentsValue = Convert.ToDecimal(RemoveInvalidCharacters(yearToDatePayments));
                    yearTodatePaymentsText = yearToDatePaymentsValue.ToString("N2");
                }
                catch (FormatException)
                {
                    yearTodatePaymentsText = RemoveInvalidCharacters(yearToDatePayments);
                }


                optionalFields.Add(new TeamsFact { Title = "Previous Payments Year To Date", Value = $"£{yearTodatePaymentsText}" });
            }
            
            if (!string.IsNullOrWhiteSpace(collectionPeriodPayments))
            {
                var collectionPeriodPaymentsText = string.Empty;
                try
                {
                    var collectionPeriodPaymentsValue = Convert.ToDecimal(RemoveInvalidCharacters(collectionPeriodPayments));
                    collectionPeriodPaymentsText = collectionPeriodPaymentsValue.ToString("N2");
                }
                catch (FormatException)
                {
                    collectionPeriodPaymentsText = RemoveInvalidCharacters(collectionPeriodPayments);
                }
                optionalFields.Add(new TeamsFact { Title = "Collection Period Payments", Value = $"£{collectionPeriodPaymentsText}" });
            }
            
            if (!string.IsNullOrEmpty(numberOfLearners))
            {
                optionalFields.Add(new TeamsFact { Title = "In Learning", Value = RemoveInvalidCharacters(numberOfLearners) });
            }

            if (!string.IsNullOrWhiteSpace(accountedForPayments))
            {
                var accountedForPaymentsText = string.Empty;
                try
                {
                    var accountedForPaymentsValue = Convert.ToDecimal(RemoveInvalidCharacters(accountedForPayments));
                    accountedForPaymentsText = accountedForPaymentsValue.ToString("N2");
                }
                catch (FormatException)
                {
                    accountedForPaymentsText = RemoveInvalidCharacters(accountedForPayments);
                }
                optionalFields.Add(new TeamsFact { Title = "Accounted For Payments", Value = $"£{accountedForPaymentsText}" });
            }

            return new TeamsCardItem
            {
                Type = "FactSet",
                Facts = optionalFields
            };
        }


        public Dictionary<string, string> ExtractAlertVariables(dynamic customMeasurements, dynamic customDimensions, DateTime timestamp)
        {
            double percentage = customMeasurements.ContainsKey("Percentage") ? customMeasurements["Percentage"] : 0;
            double duration = customMeasurements.ContainsKey("Duration") ? customMeasurements["Duration"] : 0;
            string ukprn = customDimensions.ContainsKey("Ukprn") ? customDimensions["Ukprn"] : string.Empty;
            string jobId = customDimensions.ContainsKey("JobId") ? customDimensions["JobId"] : string.Empty;
            string academicYear = customDimensions.ContainsKey("AcademicYear") ? customDimensions["AcademicYear"] : string.Empty;
            string collectionPeriod = customDimensions.ContainsKey("CollectionPeriod") ? customDimensions["CollectionPeriod"] : string.Empty;

            string dcEarningsTotal = customMeasurements.ContainsKey("DcEarningsTotal") ?
                                      customMeasurements["DcEarningsTotal"].ToString() :
                                        customMeasurements.ContainsKey("EarningsDCTotal") ?
                                        customMeasurements["EarningsDCTotal"].ToString() :
                                        string.Empty;

            string dasEarningsTotal = customMeasurements.ContainsKey("DasEarningsTotal") ?
                                       customMeasurements["DasEarningsTotal"].ToString() :
                                       string.Empty;

            string adjustedDataLockedEarnings = customMeasurements.ContainsKey("DataLockedEarningsAmount") ?
                                                 customMeasurements["DataLockedEarningsAmount"].ToString() :
                                                    customMeasurements.ContainsKey("DataLockedEarnings") ?
                                                    customMeasurements["DataLockedEarnings"].ToString() :
                                                    string.Empty;

            string differenceTotal = customMeasurements.ContainsKey("DifferenceTotal") ?
                                      customMeasurements["DifferenceTotal"].ToString() :
                                      string.Empty;

            string heldBackCompletionPayments = customMeasurements.ContainsKey("HeldBackCompletionPayments") ?
                                                 customMeasurements["HeldBackCompletionPayments"].ToString() :
                                                 string.Empty;

            string requiredPayments = customMeasurements.ContainsKey("RequiredPaymentsTotal") ?
                                       customMeasurements["RequiredPaymentsTotal"].ToString() :
                                       string.Empty;

            string collectionPeriodPayments = customMeasurements.ContainsKey("PaymentsTotal") ?
                                              customMeasurements["PaymentsTotal"].ToString() :
                                              "n/a";

            string yearToDatePayments = customMeasurements.ContainsKey("YearToDatePaymentsTotal") ?
                                         customMeasurements["YearToDatePaymentsTotal"].ToString() :
                                            customMeasurements.ContainsKey("PaymentsYearToDateTotal") ?
                                            customMeasurements["PaymentsYearToDateTotal"].ToString() :
                                            "n/a";

            string numberOfLearners = customMeasurements.ContainsKey("InLearning") ?
                                       customMeasurements["InLearning"].ToString() :
                                       "n/a";

            string accountedForPayments = customMeasurements.ContainsKey("AccountedForPayments") ?
                                            customMeasurements["AccountedForPayments"].ToString() :
                                            "n/a";

            return new Dictionary<string, string>
            {
                { "Ukprn", ukprn },
                { "JobId", jobId },
                { "AcademicYear", academicYear },
                { "CollectionPeriod", collectionPeriod },
                { "Timestamp", timestamp.ToString("F") },
                { "Accuracy", percentage >= 0 ? percentage.ToString() : string.Empty },
                { "Duration", duration > 0 ? duration.ToString() : string.Empty },
                { "DcEarningsTotal", dcEarningsTotal },
                { "DasEarningsTotal", dasEarningsTotal },
                { "AdjustedDataLockedEarnings", adjustedDataLockedEarnings },
                { "DifferenceTotal", differenceTotal },
                { "HeldBackCompletionPayments", heldBackCompletionPayments },
                { "RequiredPayments", requiredPayments },
                { "CollectionPeriodPayments", collectionPeriodPayments },
                { "YearToDatePayments", yearToDatePayments },
                { "NumberOfLearners", numberOfLearners },
                { "AccountedForPayments", accountedForPayments }
            };
        }


        public string GetAlertTitle(string alertTitleFormat, Dictionary<string, string> alertVariables)
        {
            foreach (var alertVariable in alertVariables)
            {
                alertTitleFormat = alertTitleFormat.Replace("{" + alertVariable.Key + "}", alertVariable.Value);
            }

            return alertTitleFormat;
        }

        public string GetAlertText(string alertTextFormat, Dictionary<string, string> alertVariables)
        {
            foreach (var alertVariable in alertVariables)
            {
                alertTextFormat = alertTextFormat.Replace("{" + alertVariable.Key + "}", "*" + alertVariable.Value + "*");
            }

            return alertTextFormat;
        }
    }
}