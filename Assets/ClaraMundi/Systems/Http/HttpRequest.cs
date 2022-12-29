using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace ClaraMundi
{
    public static class HttpRequest
    {
        public static async Task<T> Get<T>(string baseUrl, string url)
        {
            using var httpClient = new HttpClient();
            httpClient.BaseAddress = new Uri(baseUrl);
            httpClient.DefaultRequestHeaders.Accept.Clear();
            httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            var response = await httpClient.GetAsync(url);
            var resourceJson = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<T>(resourceJson);
        }
        public static async Task<T> Delete<T>(string baseUrl, string url)
        {
            using var httpClient = new HttpClient();
            httpClient.BaseAddress = new Uri(baseUrl);
            httpClient.DefaultRequestHeaders.Accept.Clear();
            httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            var response = await httpClient.DeleteAsync(url);
            var resourceJson = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<T>(resourceJson);
        }
        public static async Task<T> Post<T>(string baseUrl, string url, string body)
        {
            using var httpClient = new HttpClient();
            httpClient.BaseAddress = new Uri(baseUrl);
            httpClient.DefaultRequestHeaders.Accept.Clear();
            httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            var response = await httpClient.PostAsync(url, new StringContent(body, Encoding.UTF8, "application/json"));
            var resourceJson = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<T>(resourceJson);
        }
    }
}