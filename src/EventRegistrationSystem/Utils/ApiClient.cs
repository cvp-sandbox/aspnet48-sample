using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;

namespace EventRegistrationSystem.Utils
{
    public interface IApiClient
    {
        Task<T> GetAsync<T>(string endpoint, string username, string[] roles);
        Task<TResponse> PostAsync<TRequest, TResponse>(string endpoint, TRequest data, string username, string[] roles);
        // Add other methods as needed
    }

    public class ApiClient : IApiClient
    {
        private readonly HttpClient _httpClient;
        private readonly string _apiBaseUrl;

        public ApiClient(HttpClient httpClient, string apiBaseUrl)
        {
            _httpClient = httpClient;
            _apiBaseUrl = apiBaseUrl;
        }

        public async Task<T> GetAsync<T>(string endpoint, string username, string[] roles)
        {
            _httpClient.DefaultRequestHeaders.Clear();
            _httpClient.DefaultRequestHeaders.Add("X-Username", username);

            foreach (var role in roles)
            {
                _httpClient.DefaultRequestHeaders.Add("X-Role", role);
            }

            var response = await _httpClient.GetAsync($"{_apiBaseUrl}/{endpoint}");
            response.EnsureSuccessStatusCode();

            var content = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<T>(content);
        }

        public async Task<TResponse> PostAsync<TRequest, TResponse>(string endpoint, TRequest data, string username, string[] roles)
        {
            _httpClient.DefaultRequestHeaders.Clear();
            _httpClient.DefaultRequestHeaders.Add("X-Username", username);

            foreach (var role in roles)
            {
                _httpClient.DefaultRequestHeaders.Add("X-Role", role);
            }

            var jsonContent = new StringContent(
                JsonConvert.SerializeObject(data),
                System.Text.Encoding.UTF8,
                "application/json");

            var response = await _httpClient.PostAsync($"{_apiBaseUrl}/{endpoint}", jsonContent);
            response.EnsureSuccessStatusCode();

            var content = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<TResponse>(content);
        }
    }
}