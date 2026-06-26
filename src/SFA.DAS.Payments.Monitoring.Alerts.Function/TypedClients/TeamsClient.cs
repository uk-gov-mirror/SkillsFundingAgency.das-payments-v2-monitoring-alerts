using System;
using System.Net.Http;
using System.Threading.Tasks;
using System.Text.Json;

namespace SFA.DAS.Payments.Monitoring.Alerts.Function.TypedClients
{
    public class TeamsClient : ITeamsClient
    {
        private readonly HttpClient _httpClient;

        public TeamsClient(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<HttpResponseMessage> PostAsJsonAsync(string requestUrl, object jsonPayload)
        {
            if (string.IsNullOrEmpty(requestUrl)) 
            {
                throw new ArgumentNullException(nameof(requestUrl));
            }

            if (jsonPayload == null) 
            {
                throw new ArgumentNullException(nameof(jsonPayload));
            }

            var opt = new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            };

            var json = JsonSerializer.Serialize(jsonPayload, opt);

            var response = await _httpClient.PostAsJsonAsync(requestUrl, json);

            response.EnsureSuccessStatusCode();

            return response;
        }
    }
}