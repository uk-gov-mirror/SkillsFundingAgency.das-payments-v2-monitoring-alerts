using System;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Polly;
using Polly.Extensions.Http;
using SFA.DAS.Payments.Monitoring.Alerts.Function;
using SFA.DAS.Payments.Monitoring.Alerts.Function.Helpers;
using SFA.DAS.Payments.Monitoring.Alerts.Function.JsonHelpers;
using SFA.DAS.Payments.Monitoring.Alerts.Function.Services;
using SFA.DAS.Payments.Monitoring.Alerts.Function.TypedClients;

[assembly: FunctionsStartup(typeof(Startup))]

// To be done later: local settings document needs to be updated at some point with the new url and general environment variables as per the readme
// Modify slackclient to be teamsclient
// Modify slackservice


namespace SFA.DAS.Payments.Monitoring.Alerts.Function
{
    public class Startup : FunctionsStartup
    {
        private static readonly int _numberOfRetries = 4;

        public override void Configure(IFunctionsHostBuilder builder)
        {
            builder.Services.AddLogging();

            AddAppInsightsClient(builder);

            builder.Services
                .AddHttpClient<ITeamsClient, TeamsClient>();

            builder.Services.AddTransient<IDynamicJsonDeserializer, DynamicJsonDeserializer>();
            builder.Services.AddTransient<ITeamsAlertHelper, TeamsAlertHelper>();
            builder.Services.AddTransient<ITeamsService, TeamsService>();
        }

        private static void AddAppInsightsClient(IFunctionsHostBuilder builder)
        {
            builder.Services
                .AddHttpClient<IAppInsightsClient, AppInsightsClient>(x =>
                {
                    var appInsightsAPIKeyHeader = GetEnvironmentVariable("AppInsightsAuthHeader");
                    var appInsightsAPIKeyValue = GetEnvironmentVariable("AppInsightsAuthValue");

                    x.DefaultRequestHeaders.Accept.Clear();
                    x.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                    x.DefaultRequestHeaders.Add(appInsightsAPIKeyHeader, appInsightsAPIKeyValue);
                })
                .AddPolicyHandler(GetDefaultRetryPolicy());
        }

        private static IAsyncPolicy<HttpResponseMessage> GetDefaultRetryPolicy()
        {
            return HttpPolicyExtensions
                .HandleTransientHttpError()
                .OrResult(msg => msg.StatusCode == HttpStatusCode.NotFound)
                .WaitAndRetryAsync(
                    _numberOfRetries,
                    retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)),
                    (outcome, timespan, retryAttempt, context) =>
                    {
                        var log = context.GetLogger();
                        log?.LogInformation(
                            $"Request failed with status code {outcome.Result.StatusCode} delaying for {timespan.TotalMilliseconds} milliseconds then retry {retryAttempt}");
                    });
        }

        private static string GetEnvironmentVariable(string variableName)
        {
            return Environment.GetEnvironmentVariable(variableName, EnvironmentVariableTarget.Process);
        }
    }
}