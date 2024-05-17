using Polly.Retry;
using Polly;

namespace CurrencyConverter.Utils
{
    public class HttpAPIClient: IHttpAPIClient
    {
        private readonly HttpClient _httpClient;
        private readonly AsyncRetryPolicy<HttpResponseMessage> _retryPolicy;
        private readonly IConfiguration _config;
        public HttpAPIClient(HttpClient httpClient, IConfiguration config)
        {
            _httpClient = httpClient;
            _config = config;
            _httpClient.BaseAddress = new Uri(_config.GetValue<string>("URL:Base"));

            _retryPolicy = Policy<HttpResponseMessage>
            .Handle<HttpRequestException>()
            .OrResult(response => !response.IsSuccessStatusCode)
            .WaitAndRetryAsync(3, attempt => TimeSpan.FromSeconds(Math.Pow(2, attempt)));
        }

        public async Task<HttpResponseMessage> SendRequestAsync(string url)
        {
            // Send request using retry policy
            return await _retryPolicy.ExecuteAsync(async () =>
            {
                return await _httpClient.GetAsync(url);
            });
        }
    }
}