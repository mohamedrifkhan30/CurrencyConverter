using Polly.Retry;
using Polly;
using Microsoft.Extensions.Caching.Memory;

namespace CurrencyConverter.Utils
{
    public class HttpAPIClient: IHttpAPIClient
    {
        private readonly HttpClient _httpClient;
        private readonly AsyncRetryPolicy<HttpResponseMessage> _retryPolicy;
        private readonly IConfiguration _config;
        private readonly IMemoryCache _cache;
        private static readonly TimeSpan CacheDuration = TimeSpan.FromMinutes(10);
        public HttpAPIClient(HttpClient httpClient, IConfiguration config, IMemoryCache cache)
        {
            _httpClient = httpClient;
            _config = config;
            _cache = cache;
            _httpClient.BaseAddress = new Uri(_config.GetValue<string>("URL:Base"));

            _retryPolicy = Policy<HttpResponseMessage>
            .Handle<HttpRequestException>()
            .OrResult(response => !response.IsSuccessStatusCode)
            .WaitAndRetryAsync(3, attempt => TimeSpan.FromSeconds(Math.Pow(2, attempt)));
        }

        public async Task<HttpResponseMessage> SendRequestAsync(string url)
        {
            if (_cache.TryGetValue(url, out HttpResponseMessage cachedResponse))
            {
                return cachedResponse;
            }
            // Send request using retry policy
            var response = await _retryPolicy.ExecuteAsync(() => _httpClient.GetAsync(url));
            
            if (response.IsSuccessStatusCode)
            {
                _cache.Set(url, response, CacheDuration);
            }

            return response;
        }
    }
}