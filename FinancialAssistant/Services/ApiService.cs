using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace FinancialAssistant.Services
{
    public class ApiService
    {
        private readonly HttpClient _httpClient;

        public ApiService(string baseUrl)
        {
            _httpClient = new HttpClient
            {
                BaseAddress = new Uri(baseUrl)
            };
            _httpClient.DefaultRequestHeaders.Accept.Clear();
            _httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        }

        // Метод для выполнения GET-запроса
        public async Task<T> GetAsync<T>(string endpoint)
        {
            HttpResponseMessage response = await _httpClient.GetAsync(endpoint);
            if (response.IsSuccessStatusCode)
            {
                var jsonResponse = await response.Content.ReadAsStringAsync();
                return JsonConvert.DeserializeObject<T>(jsonResponse);
            }
            else
            {
                throw new Exception($"Ошибка при выполнении GET-запроса: {response.StatusCode}");
            }
        }

        // Метод для выполнения POST-запроса
        public async Task<T> PostAsync<T>(string endpoint, T data)
        {
            var jsonData = JsonConvert.SerializeObject(data);
            var content = new StringContent(jsonData, Encoding.UTF8, "application/json");

            HttpResponseMessage response = await _httpClient.PostAsync(endpoint, content);
            if (response.IsSuccessStatusCode)
            {
                var jsonResponse = await response.Content.ReadAsStringAsync();
                return JsonConvert.DeserializeObject<T>(jsonResponse);
            }
            else
            {
                throw new Exception($"Ошибка при выполнении POST-запроса: {response.StatusCode}");
            }
        }

        // Метод для выполнения PUT-запроса
        public async Task PutAsync<T>(string endpoint, T data)
        {
            var jsonData = JsonConvert.SerializeObject(data);
            var content = new StringContent(jsonData, Encoding.UTF8, "application/json");

            HttpResponseMessage response = await _httpClient.PutAsync(endpoint, content);
            if (!response.IsSuccessStatusCode)
            {
                throw new Exception($"Ошибка при выполнении PUT-запроса: {response.StatusCode}");
            }
        }

        // Метод для выполнения DELETE-запроса
        public async Task DeleteAsync(string endpoint)
        {
            HttpResponseMessage response = await _httpClient.DeleteAsync(endpoint);
            if (!response.IsSuccessStatusCode)
            {
                throw new Exception($"Ошибка при выполнении DELETE-запроса: {response.StatusCode}");
            }
        }
    }
}
