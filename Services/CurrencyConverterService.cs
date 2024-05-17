using CurrencyConverter.Models;
using CurrencyConverter.Utils;
using System.Linq;
using System.Text.Json;

namespace CurrencyConverter.Services
{
    public class CurrencyConverterService : ICurrencyConverterService
    {
        private readonly IConfiguration _config;
        private readonly IHttpAPIClient _httpAPIClient;
        public CurrencyConverterService(
            IConfiguration config,
            IHttpAPIClient httpAPIClient
            )
        {
            _config = config;
            _httpAPIClient = httpAPIClient;

        }

        /// <summary>
        ///  Convert Amount method
        /// </summary>
        /// <param name="fromCurrency"></param>
        /// <param name="toCurrency"></param>
        /// <param name="amount"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public async Task<ExchangeRate?> ConvertAmount(string fromCurrency, string toCurrency, double amount)
        {
            var response = await _httpAPIClient.SendRequestAsync(_config.GetValue<string>("URL:Latest") + $"?amount={amount}&from={fromCurrency}&to={toCurrency}");


            if (!response.IsSuccessStatusCode)
                throw new Exception($"Failed to fetch data from the API");

            response.EnsureSuccessStatusCode();
            var responseBody = await response.Content.ReadAsStringAsync();
            var exchangeRates = JsonSerializer.Deserialize<ExchangeRate>(responseBody);

            return exchangeRates;
        }

        public async Task<HistoricalRates?> GetHistoricalRates(DateTime startDate, DateTime endDate, string baseCurrency)
        {
            var response = await _httpAPIClient.SendRequestAsync($"/{startDate.ToString("yyyy-MM-dd")}..{endDate.ToString("yyyy-MM-dd")}?to={baseCurrency}");
            if (!response.IsSuccessStatusCode)
                throw new Exception($"Failed to fetch data from the API");

            response.EnsureSuccessStatusCode();
            var responseBody = await response.Content.ReadAsStringAsync();
            var historicalRates = JsonSerializer.Deserialize<HistoricalRates>(responseBody);
            // throw new NotImplementedException();

            return historicalRates!;
        }

        /// <summary>
        ///  Get Latest Rates Async http Client Extrenal Call
        /// </summary>
        /// <param name="baseCurrency"></param>
        /// <returns></returns>
        public async Task<ExchangeRate?> GetLatestRatesAsync(string baseCurrency)
        {
            var response = await _httpAPIClient.SendRequestAsync(_config.GetValue<string>("URL:Latest") + $"?base={baseCurrency}");
            if (!response.IsSuccessStatusCode)
                throw new Exception($"Failed to fetch data from the API for {baseCurrency.ToUpper()}");
            response.EnsureSuccessStatusCode();
            var responseBody = await response.Content.ReadAsStringAsync();

            var exchangeRates = JsonSerializer.Deserialize<ExchangeRate>(responseBody);

            return exchangeRates;
        }
    }
}