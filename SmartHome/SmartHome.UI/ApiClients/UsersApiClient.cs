using Newtonsoft.Json;
using SmartHome.UI.Data;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Mime;
using System.Text;
using System.Threading.Tasks;

namespace SmartHome.UI.ApiClients
{
    public class UsersApiClient:ApiClient
    {
        public UsersApiClient(string apiUrl) : base(apiUrl)
        {
        }

        public async Task<bool> AddUser(UserModel user)
        {
            var request = new HttpRequestMessage
            {
                Method = HttpMethod.Post,
                RequestUri = new Uri($"{HttpClient.BaseAddress}Users"),
                Content = new StringContent(JsonConvert.SerializeObject(user), Encoding.UTF8, MediaTypeNames.Application.Json),
            };
            var response = await HttpClient.SendAsync(request);
            return response.IsSuccessStatusCode;
        }

        public async Task<bool> AddGroup(Group group)
        {
            var request = new HttpRequestMessage
            {
                Method = HttpMethod.Post,
                RequestUri = new Uri($"{HttpClient.BaseAddress}Groups"),
                Content = new StringContent(JsonConvert.SerializeObject(group), Encoding.UTF8, MediaTypeNames.Application.Json),
            };
            var response = await HttpClient.SendAsync(request);
            return response.IsSuccessStatusCode;
        }

        public async Task<bool> AddFriend(UserFriendModel model)
        {
            var request = new HttpRequestMessage
            {
                Method = HttpMethod.Post,
                RequestUri = new Uri($"{HttpClient.BaseAddress}Users/AddFriend"),
                Content = new StringContent(JsonConvert.SerializeObject(model), Encoding.UTF8, MediaTypeNames.Application.Json),
            };
            var response = await HttpClient.SendAsync(request).ConfigureAwait(false);
            return response.IsSuccessStatusCode;
        }

        public async Task<bool> RemoveFriend(UserFriendModel model)
        {
            var request = new HttpRequestMessage
            {
                Method = HttpMethod.Delete,
                RequestUri = new Uri($"{HttpClient.BaseAddress}Users/RemoveFriend"),
                Content = new StringContent(JsonConvert.SerializeObject(model), Encoding.UTF8, MediaTypeNames.Application.Json),
            };
            var response = await HttpClient.SendAsync(request).ConfigureAwait(false);
            return response.IsSuccessStatusCode;
        }

        public async Task<bool> AddClaim(GroupResourceModel model)
        {
            var request = new HttpRequestMessage
            {
                Method = HttpMethod.Post,
                RequestUri = new Uri($"{HttpClient.BaseAddress}Groups/AddClaimToGroup"),
                Content = new StringContent(JsonConvert.SerializeObject(model), Encoding.UTF8, MediaTypeNames.Application.Json),
            };
            var response = await HttpClient.SendAsync(request).ConfigureAwait(false);
            return response.IsSuccessStatusCode;
        }

        public async Task<bool> RemoveClaim(GroupResourceModel model)
        {
            var request = new HttpRequestMessage
            {
                Method = HttpMethod.Delete,
                RequestUri = new Uri($"{HttpClient.BaseAddress}Groups/RemoveFriend"),
                Content = new StringContent(JsonConvert.SerializeObject(model), Encoding.UTF8, MediaTypeNames.Application.Json),
            };
            var response = await HttpClient.SendAsync(request).ConfigureAwait(false);
            return response.IsSuccessStatusCode;
        }

        public async Task<bool> RemoveGroup(string groupId)
        {
            var request = new HttpRequestMessage
            {
                Method = HttpMethod.Delete,
                RequestUri = new Uri($"{HttpClient.BaseAddress}Groups/{groupId}")
            };
            var response = await HttpClient.SendAsync(request).ConfigureAwait(false);
            return response.IsSuccessStatusCode;
        }

        public async Task<List<UserModel>> GetFriends(string userId)
        {
            var response = await HttpClient.GetAsync($"Users/GetFriends/{userId}").ConfigureAwait(false);
            if (response.IsSuccessStatusCode)
            {
                var responseString = await response.Content.ReadAsStringAsync();
                var concepts = JsonConvert.DeserializeObject<List<UserModel>>(responseString);
                return concepts;
            }
            else
            {
                return new List<UserModel>();
            }
        }

        public async Task<List<Group>> GetGroups(string userId)
        {
            var response = await HttpClient.GetAsync($"Groups/GetGroupsByOnwer/{userId}").ConfigureAwait(false);
            if (response.IsSuccessStatusCode)
            {
                var responseString = await response.Content.ReadAsStringAsync();
                var group = JsonConvert.DeserializeObject<List<Group>>(responseString);
                return group;
            }
            else
            {
                return new List<Group>();
            }
        }

        public async Task<GroupWithData> GetGroupWithUsers(string groupId)
        {
            var response = await HttpClient.GetAsync($"Groups/GetWithUsers/{groupId}").ConfigureAwait(false);
            if (response.IsSuccessStatusCode)
            {
                var responseString = await response.Content.ReadAsStringAsync();
                var groups = JsonConvert.DeserializeObject<GroupWithData>(responseString);
                return groups;
            }
            else
            {
                return null;
            }
        }

        public async Task<List<UserModel>> GetAvailableFriends(string userId)
        {
            var response = await HttpClient.GetAsync($"Users/AvailableFriends/{userId}").ConfigureAwait(false);
            if (response.IsSuccessStatusCode)
            {
                var responseString = await response.Content.ReadAsStringAsync();
                var concepts = JsonConvert.DeserializeObject<List<UserModel>>(responseString);
                return concepts;
            }
            else
            {
                return new List<UserModel>();
            }
        }

        public async Task<bool> AddUserToGroup(UserGroupModel model)
        {
            var request = new HttpRequestMessage
            {
                Method = HttpMethod.Post,
                RequestUri = new Uri($"{HttpClient.BaseAddress}Groups/AddUserToGroup"),
                Content = new StringContent(JsonConvert.SerializeObject(model), Encoding.UTF8, MediaTypeNames.Application.Json),
            };
            var response = await HttpClient.SendAsync(request).ConfigureAwait(false);
            return response.IsSuccessStatusCode;
        }

        public async Task<bool> RemoveUserFromGroup(UserGroupModel model)
        {
            var request = new HttpRequestMessage
            {
                Method = HttpMethod.Delete,
                RequestUri = new Uri($"{HttpClient.BaseAddress}Groups/RemoveUserFromGroup"),
                Content = new StringContent(JsonConvert.SerializeObject(model), Encoding.UTF8, MediaTypeNames.Application.Json),
            };
            var response = await HttpClient.SendAsync(request).ConfigureAwait(false);
            return response.IsSuccessStatusCode;
        }
    }
}
