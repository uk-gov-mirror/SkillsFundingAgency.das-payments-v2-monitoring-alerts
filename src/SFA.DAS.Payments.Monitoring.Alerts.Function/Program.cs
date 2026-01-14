using System;
using System.Net;
using System.Net.Http.Headers;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Polly;
using Polly.Extensions.Http;
using SFA.DAS.Payments.Monitoring.Alerts.Function.Helpers;
using SFA.DAS.Payments.Monitoring.Alerts.Function.JsonHelpers;
using SFA.DAS.Payments.Monitoring.Alerts.Function.Services;
using SFA.DAS.Payments.Monitoring.Alerts.Function.TypedClients;

var host = new HostBuilder()
    .ConfigureFunctionsWebApplication()
    .ConfigureFunctionsWorkerDefaults()
    .ConfigureServices(services =>
    {
        services.AddLogging();

        // AppInsightsClient registration
        services
            .AddHttpClient<IAppInsightsClient, AppInsightsClient>(x =>
            {
                var appInsightsAPIKeyHeader = GetEnvironmentVariable("AppInsightsAuthHeader");
                var appInsightsAPIKeyValue = GetEnvironmentVariable("AppInsightsAuthValue");

                x.DefaultRequestHeaders.Accept.Clear();
                x.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                x.DefaultRequestHeaders.Add(appInsightsAPIKeyHeader, appInsightsAPIKeyValue);
            })
            .AddPolicyHandler(GetDefaultRetryPolicy());

        // SlackClient registration
        services
            .AddHttpClient<ISlackClient, SlackClient>(x =>
            {
                x.BaseAddress = new Uri(GetEnvironmentVariable("SlackBaseUrl"));
            });

        services.AddTransient<IDynamicJsonDeserializer, DynamicJsonDeserializer>();
        services.AddTransient<ISlackAlertHelper, SlackAlertHelper>();
        services.AddTransient<ISlackService, SlackService>();
    })
    .Build();

host.Run();

static IAsyncPolicy<HttpResponseMessage> GetDefaultRetryPolicy()
{
    const int numberOfRetries = 4;
    return HttpPolicyExtensions
        .HandleTransientHttpError()
        .OrResult(msg => msg.StatusCode == HttpStatusCode.NotFound)
        .WaitAndRetryAsync(
            numberOfRetries,
            retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)),
            (outcome, timespan, retryAttempt, context) =>
            {
                // Logging is not directly available here in isolated worker
                // Consider injecting ILogger if needed elsewhere
            });
}

static string GetEnvironmentVariable(string variableName)
{
    return Environment.GetEnvironmentVariable(variableName, EnvironmentVariableTarget.Process);
}
