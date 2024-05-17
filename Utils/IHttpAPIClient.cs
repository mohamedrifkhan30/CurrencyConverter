namespace CurrencyConverter.Utils
{
    public interface IHttpAPIClient
    {
        Task<HttpResponseMessage> SendRequestAsync(string url);
    }
}
