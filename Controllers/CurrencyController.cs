using CurrencyConverter.Models;
using CurrencyConverter.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.VisualBasic;
using System.Net.Http;

namespace CurrencyConverter.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CurrencyController : ControllerBase
    {
        
        private readonly ICurrencyConverterService _currencyService;

        public CurrencyController(ICurrencyConverterService currencyService)
        {
            _currencyService = currencyService;
        }

        /// <summary>
        /// GetLatestExchangeRates get endpoint /latest
        /// </summary>
        /// <param name="baseCurrency"></param>
        /// <returns></returns>
        [HttpGet("latest")]
        public async Task<ActionResult<ExchangeRate>> GetLatestExchangeRates([BindRequired] string baseCurrency)
        {
            try
            {
                return Ok(await _currencyService.GetLatestRatesAsync(baseCurrency.Trim()));
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }


        [HttpGet("convert")]
        public async Task<ActionResult<double>> ConvertCurrency([BindRequired] string fromCurrency, [BindRequired] string toCurrency,[BindRequired] double amount)
        {
            try
            {
                if (IsExcludedCurrency(toCurrency) || IsExcludedCurrency(fromCurrency))
                {
                    return BadRequest("Conversion not supported for this currency");
                }

                var exchangeRates = await _currencyService.ConvertAmount(fromCurrency, toCurrency, amount);

                double exchangeRate = exchangeRates!.rates![toCurrency.ToUpper()];
                
                return Ok(amount * exchangeRate);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }


        [HttpGet("historical")]
        public async Task<ActionResult<HistoricalRates>> GetHistoricalRates([BindRequired] DateTime startDate, [BindRequired] DateTime endDate, [BindRequired] string baseCurrency,int page = 1, int pageSize = 3)
        {

           var responsed = await _currencyService.GetHistoricalRates(startDate, endDate, baseCurrency);
            if(responsed != null)
            {
                var rates = responsed?.rates?.Skip((page - 1) * pageSize).Take(pageSize).ToDictionary(x => x.Key, x => x.Value);
                responsed!.rates = rates;
            }
            
            return responsed!;
        }

        /// <summary>
        ///  Private Validation Method
        /// </summary>
        /// <param name="currency"></param>
        /// <returns></returns>
        private bool IsExcludedCurrency(string currency)
        {
            return new[] { "TRY", "PLN", "THB", "MXN" }.Contains(currency);
        }

    }
}
