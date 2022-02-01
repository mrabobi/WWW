using System;
using System.Net.Http;

namespace SmartHome.UI.ApiClients
{
    public class ApiClient
    {
        protected readonly HttpClient HttpClient;

        protected ApiClient(string apiUrl)
        {
            HttpClient = new HttpClient() { BaseAddress = new Uri(apiUrl) };
        }

        protected ApiClient(string apiUrl, HttpClientHandler httpClientHandler)
        {
            HttpClient = new HttpClient(httpClientHandler) { BaseAddress = new Uri(apiUrl) };
        }

        public string GetBaseUrl()
        {
            return HttpClient?.BaseAddress.ToString();
        }
    }
}
