using System.Net.Http;
using System.Text;
using Newtonsoft.Json;

namespace OnlineStore.Mobile.Services
{
    public class ApiService
    {
        private readonly HttpClient _httpClient;

        public ApiService()
        {
            var handler = new HttpClientHandler
            {
                ServerCertificateCustomValidationCallback = (message, cert, chain, sslPolicyErrors) => true
            };

            _httpClient = new HttpClient(handler)
            {
                BaseAddress = new Uri("https://10.0.2.2:7190"),
                Timeout = TimeSpan.FromSeconds(30)
            };

        }

        public async Task<List<T>> GetDataAsync<T>(string endpoint)
        {
            try
            {
                Console.WriteLine($"Connecting to API: {endpoint}");
                var response = await _httpClient.GetAsync(endpoint);
                response.EnsureSuccessStatusCode();

                Console.WriteLine($"API Connection Successful: {endpoint}");
                var json = await response.Content.ReadAsStringAsync();

                Console.WriteLine($"API Response: {json}");
                return JsonConvert.DeserializeObject<List<T>>(json);
            }
            catch (HttpRequestException ex)
            {
                Console.WriteLine($"HTTP Request Exception: {ex.Message}");
                if (ex.InnerException != null)
                {
                    Console.WriteLine($"Inner Exception: {ex.InnerException.Message}");
                }
                return new List<T>();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"General Exception: {ex.Message}");
                return new List<T>();
            }
        }



    }
}



