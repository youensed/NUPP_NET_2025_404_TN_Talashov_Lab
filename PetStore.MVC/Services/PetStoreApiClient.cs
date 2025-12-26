using Newtonsoft.Json;
using System.Text;

namespace PetStore.MVC.Services
{
    public class PetStoreApiClient : IPetStoreApiClient
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<PetStoreApiClient> _logger;

        public PetStoreApiClient(HttpClient httpClient, ILogger<PetStoreApiClient> logger)
        {
            _httpClient = httpClient;
            _logger = logger;
        }

        public async Task<T?> GetAsync<T>(string endpoint)
        {
            try
            {
                var response = await _httpClient.GetAsync(endpoint);
                
                if (!response.IsSuccessStatusCode)
                {
                    _logger.LogWarning($"GET {endpoint} returned {response.StatusCode}");
                    return default;
                }

                var content = await response.Content.ReadAsStringAsync();
                return JsonConvert.DeserializeObject<T>(content);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error getting data from {endpoint}");
                throw;
            }
        }

        public async Task<IEnumerable<T>> GetListAsync<T>(string endpoint)
        {
            try
            {
                var response = await _httpClient.GetAsync(endpoint);
                
                if (!response.IsSuccessStatusCode)
                {
                    _logger.LogWarning($"GET {endpoint} returned {response.StatusCode}");
                    return Enumerable.Empty<T>();
                }

                var content = await response.Content.ReadAsStringAsync();
                var result = JsonConvert.DeserializeObject<IEnumerable<T>>(content);
                return result ?? Enumerable.Empty<T>();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error getting list from {endpoint}");
                return Enumerable.Empty<T>();
            }
        }

        public async Task<bool> PostAsync<T>(string endpoint, T data)
        {
            try
            {
                var json = JsonConvert.SerializeObject(data);
                var content = new StringContent(json, Encoding.UTF8, "application/json");
                
                var response = await _httpClient.PostAsync(endpoint, content);
                return response.IsSuccessStatusCode;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error posting data to {endpoint}");
                return false;
            }
        }

        public async Task<bool> PutAsync<T>(string endpoint, T data)
        {
            try
            {
                var json = JsonConvert.SerializeObject(data);
                var content = new StringContent(json, Encoding.UTF8, "application/json");
                
                var response = await _httpClient.PutAsync(endpoint, content);
                return response.IsSuccessStatusCode;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error updating data at {endpoint}");
                return false;
            }
        }

        public async Task<bool> DeleteAsync(string endpoint)
        {
            try
            {
                var response = await _httpClient.DeleteAsync(endpoint);
                return response.IsSuccessStatusCode;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error deleting data at {endpoint}");
                return false;
            }
        }
    }
}

