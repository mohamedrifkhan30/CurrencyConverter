using System.Text.Json.Serialization;

namespace CurrencyConverter.Models
{
    public class HistoricalRates
    {
        public double amount {  get; set; }
        [JsonPropertyName("base")]
        public string? baseCurrency { get; set; }
        public string start_date { get; set; }
        public string end_date { get; set; }
        public Dictionary<string,Dictionary<string,double>>? rates { get; set; }
    }
}
