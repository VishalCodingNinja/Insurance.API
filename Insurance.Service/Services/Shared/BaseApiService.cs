using Newtonsoft.Json;

namespace Insurance.Business.Domain.Services.Shared
{
    public abstract class BaseApiService
    {
        private readonly IHttpClientFactory _httpClientFactory;

        protected BaseApiService(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        protected async Task<T?> FetchFromApiAsync<T>(string route, string clientName) where T : class
        {
            try
            {
                var client = _httpClientFactory.CreateClient(clientName);
                var response = await client.GetAsync(route);

                if (!response.IsSuccessStatusCode)
                {
                    // Log the error 
                    Console.WriteLine($"Request to {route} failed with status code {response.StatusCode}");
                    return null;
                }

                var responseBody = await response.Content.ReadAsStringAsync();
                return JsonConvert.DeserializeObject<T>(responseBody);
            }
            catch (Exception ex)
            {
                // Log the exception
                Console.WriteLine($"An error occurred while fetching data from API: {ex.Message}");
                return null;
            }
        }
    }

}
