using System.Net.Http.Json;

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

            var response = await _httpClient.PostAsJsonAsync(requestUrl, jsonPayload);

            response.EnsureSuccessStatusCode();

            return response;
        }
    }
}