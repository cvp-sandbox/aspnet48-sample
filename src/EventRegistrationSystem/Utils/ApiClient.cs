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
    Task<TResponse> PostAsync<TRequest, TResponse>(string endpoint, TRequest data, string username, string[] roles, HttpMethod method = null);
    Task DeleteAsync(string endpoint, string username, string[] roles);
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

    public async Task<TResponse> PostAsync<TRequest, TResponse>(string endpoint, TRequest data, string username, string[] roles, HttpMethod method = null)
    {
        _httpClient.DefaultRequestHeaders.Clear();
        _httpClient.DefaultRequestHeaders.Add("X-Username", username);

        foreach (var role in roles)
        {
            _httpClient.DefaultRequestHeaders.Add("X-Role", role);
        }

        var jsonContent = data != null ? 
            new StringContent(
                JsonConvert.SerializeObject(data),
                System.Text.Encoding.UTF8,
                "application/json") : 
            null;

        HttpResponseMessage response;
        
        if (method == HttpMethod.Put)
        {
            response = await _httpClient.PutAsync($"{_apiBaseUrl}/{endpoint}", jsonContent);
        }
        else
        {
            response = await _httpClient.PostAsync($"{_apiBaseUrl}/{endpoint}", jsonContent);
        }
        
        response.EnsureSuccessStatusCode();

        var content = await response.Content.ReadAsStringAsync();
        return JsonConvert.DeserializeObject<TResponse>(content);
    }
    
    public async Task DeleteAsync(string endpoint, string username, string[] roles)
    {
        _httpClient.DefaultRequestHeaders.Clear();
        _httpClient.DefaultRequestHeaders.Add("X-Username", username);

        foreach (var role in roles)
        {
            _httpClient.DefaultRequestHeaders.Add("X-Role", role);
        }

        var response = await _httpClient.DeleteAsync($"{_apiBaseUrl}/{endpoint}");
        response.EnsureSuccessStatusCode();
    }
    }
}
