using System;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace ClaraMundi.Http
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
            if (response.StatusCode != HttpStatusCode.OK)
                throw new Exception(response.StatusCode + "");
            var resourceJson = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<T>(resourceJson);
        }
    }
}