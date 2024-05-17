using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace CurrencyConverter.Models
{
    public class ExchangeRate
    {
        public double amount { get; set; }
        [JsonPropertyName("base")]
        public string? baseCurrency { get; set; }
        public DateTime? date { get; set; }
        public Dictionary<string, double>? rates { get; set; }
    }
}