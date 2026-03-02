using System.Text.Json;
using Microsoft.Extensions.Caching.Memory;

namespace InternetShop.Services
{
    public class CoinGeckoService
    {
        private readonly HttpClient _httpClient;
        private readonly IMemoryCache _cache;
        private const string CacheKey = "EthUsdPrice";

        public CoinGeckoService(HttpClient httpClient, IMemoryCache cache)
        {
            _httpClient = httpClient;
            _cache = cache;
        }

        public async Task<decimal> GetEthToUsdRateAsync()
        {
            if (_cache.TryGetValue(CacheKey, out decimal cachedPrice))
            {
                return cachedPrice;
            }

            try
            {
                var response = await _httpClient.GetAsync("https://api.coingecko.com/api/v3/simple/price?ids=ethereum&vs_currencies=usd");
                response.EnsureSuccessStatusCode();

                var jsonString = await response.Content.ReadAsStringAsync();
                using var document = JsonDocument.Parse(jsonString);

                var price = document.RootElement.GetProperty("ethereum").GetProperty("usd").GetDecimal();

                var cacheOptions = new MemoryCacheEntryOptions()
                    .SetAbsoluteExpiration(TimeSpan.FromMinutes(5));

                _cache.Set(CacheKey, price, cacheOptions);

                return price;
            }
            catch (Exception)
            {
                return 3000.00m;
            }
        }
    }
}