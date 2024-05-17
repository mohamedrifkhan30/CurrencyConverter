using CurrencyConverter.Models;

namespace CurrencyConverter.Services
{
    public interface ICurrencyConverterService
    {
        public Task<ExchangeRate?> GetLatestRatesAsync(string baseCurrency);
        public Task<ExchangeRate?> ConvertAmount(string fromCurrency, string toCurrency, double amount);
        public Task<HistoricalRates?> GetHistoricalRates(DateTime startDate, DateTime endDate, string baseCurrency);
    }
}