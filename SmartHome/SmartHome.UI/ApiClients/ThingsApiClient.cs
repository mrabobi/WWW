using Newtonsoft.Json;
using SmartHome.UI.Data;
using SmartHome.UI_Auth.Utils;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Mime;
using System.Text;
using System.Threading.Tasks;

namespace SmartHome.UI.ApiClients
{
    public class ThingsApiClient:ApiClient
    {
        public ThingsApiClient(string apiUrl) : base(apiUrl)
        {
        }

        public async Task<List<Thing>> GetAccesibleThings(string userId)
        {
            var response = await HttpClient.GetAsync($"Things/GetAccessible/{userId}").ConfigureAwait(false);
            if (response.IsSuccessStatusCode)
            {
                var responseString = await response.Content.ReadAsStringAsync();
                var group = JsonConvert.DeserializeObject<List<Thing>>(responseString);
                return group;
            }
            else
            {
                return new List<Thing>();
            }
        }

        public async Task<bool> AddThing(Thing thing)
        {
            thing.ownerId = AppState.CurrentUser.UserId;
            var request = new HttpRequestMessage
            {
                Method = HttpMethod.Post,
                RequestUri = new Uri($"{HttpClient.BaseAddress}Things"),
                Content = new StringContent(JsonConvert.SerializeObject(thing), Encoding.UTF8, MediaTypeNames.Application.Json),
            };
            var response = await HttpClient.SendAsync(request);
            return response.IsSuccessStatusCode;
        }

        public async Task<bool> ActivateThing(ThingViewModel model)
        {
            var request = new HttpRequestMessage
            {
                Method = HttpMethod.Post,
                RequestUri = new Uri(model.validation_url),
                Content = new StringContent(JsonConvert.SerializeObject(new ValueObject { value=model.access_code }), Encoding.UTF8, MediaTypeNames.Application.Json),
            };
            var response = await HttpClient.SendAsync(request);
            if(!response.IsSuccessStatusCode)
            {
                return false;
            }
            var responseString = await response.Content.ReadAsStringAsync();
            var thing = JsonConvert.DeserializeObject<Thing>(responseString);
            return await AddThing(thing);
        }

        public async Task<List<ThingViewModel>> ScanThings(string apiUrl)
        {
            var request = new HttpRequestMessage
            {
                Method = HttpMethod.Get,
                RequestUri = new Uri(apiUrl),
            };
            var response = await HttpClient.SendAsync(request);
            if (response.IsSuccessStatusCode)
            {
                var responseString = await response.Content.ReadAsStringAsync();
                var group = JsonConvert.DeserializeObject<List<ThingViewModel>>(responseString);
                return group;
            }
            else
            {
                return new List<ThingViewModel>();
            }
        }

        public async Task<string> ReadProperty(string apiUrl)
        {
            var request = new HttpRequestMessage
            {
                Method = HttpMethod.Get,
                RequestUri = new Uri(apiUrl),
            };
            var response = await HttpClient.SendAsync(request);
            if (response.IsSuccessStatusCode)
            {
                var responseString = await response.Content.ReadAsStringAsync();
                var valueObject = JsonConvert.DeserializeObject<ValueObject>(responseString);
                return valueObject.value;
            }
            else
            {
                return null;
            }
        }

        public async Task<bool> WriteProperty(string apiUrl,string valueString)
        {
            var request = new HttpRequestMessage
            {
                Method = HttpMethod.Post,
                RequestUri = new Uri(apiUrl),
                Content = new StringContent(JsonConvert.SerializeObject(new ValueObject { value = valueString }), Encoding.UTF8, MediaTypeNames.Application.Json),
            };
            var response = await HttpClient.SendAsync(request);
            return response.IsSuccessStatusCode;
        }

        public async Task<string> DoAction(string apiUrl,Dictionary<string,string> parameters)
        {
            var request = new HttpRequestMessage
            {
                Method = HttpMethod.Post,
                RequestUri = new Uri(apiUrl),
                Content = new StringContent(JsonConvert.SerializeObject(parameters), Encoding.UTF8, MediaTypeNames.Application.Json),
            };
            var response = await HttpClient.SendAsync(request);
            if (response.IsSuccessStatusCode)
            {
                var responseString = await response.Content.ReadAsStringAsync();
                var valueObject = JsonConvert.DeserializeObject<ValueObject>(responseString);
                return valueObject.value;
            }
            else
            {
                return null;
            }
        }

        public async Task<bool> DoActionVoid(string apiUrl, Dictionary<string, string> parameters)
        {
            var request = new HttpRequestMessage
            {
                Method = HttpMethod.Post,
                RequestUri = new Uri(apiUrl),
                Content = new StringContent(JsonConvert.SerializeObject(parameters), Encoding.UTF8, MediaTypeNames.Application.Json),
            };
            var response = await HttpClient.SendAsync(request);
            return response.IsSuccessStatusCode;
        }
    }
}
