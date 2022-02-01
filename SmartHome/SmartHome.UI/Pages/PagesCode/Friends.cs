using Microsoft.AspNetCore.Components;
using MudBlazor;
using SmartHome.UI.ApiClients;
using SmartHome.UI.Data;
using SmartHome.UI_Auth.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SmartHome.UI.Pages
{
    public partial class Friends
    {
        [Inject]
        public UsersApiClient ApiClient { get; set; }
        [Inject]
        public ISnackbar SnackBar { get; set; }
        public List<UserModel> AvailableFriends { get; set; }
        public List<UserModel> FriendsList { get; set; }
        public List<string> AvaiableFriendsDisplayNames => AvailableFriends?.Select(u => u.DisplayName).ToList();
        public bool DataIsLoading { get; set; }
        public string SelectedValue { get; set; }
        private string searchString1 = "";

        protected override async Task OnInitializedAsync()
        {
            DataIsLoading = true;
            AvailableFriends = await ApiClient.GetAvailableFriends(AppState.CurrentUser.UserId);
            FriendsList = await ApiClient.GetFriends(AppState.CurrentUser.UserId);
            DataIsLoading = false;
        }

        private async Task<IEnumerable<string>> Search1(string value)
        {
            await Task.Delay(5);

            if (string.IsNullOrEmpty(value))
            {
                return AvaiableFriendsDisplayNames;
            }
            return AvaiableFriendsDisplayNames.Where(x => x.Contains(value, StringComparison.InvariantCultureIgnoreCase));
        }

        private bool FilterFunc1(UserModel element) => FilterFunc(element, searchString1);

        private bool FilterFunc(UserModel element, string searchString)
        {
            if (string.IsNullOrWhiteSpace(searchString))
            {
                return true;
            }

            if ($"{element.DisplayName} {element.FirstName} {element.LastName}".Contains(searchString))
            {
                return true;
            }
            return false;
        }

        private async Task DeleteFriend(string userId)
        {
            var currentUser = AppState.CurrentUser.UserId;
            var success = await ApiClient.RemoveFriend(new UserFriendModel { FirstUserId = currentUser, SecondUserId = userId });
            if(!success)
            {
                SnackBar.Add("Could not delete friend", Severity.Error);
            }
            else
            {
                var deletedFriend = FriendsList.FirstOrDefault(user => user.UserId == userId);
                AvailableFriends.Add(deletedFriend);
                FriendsList = FriendsList.Where(user => user.UserId != userId).ToList();
                SnackBar.Add("Friend deleted", Severity.Success);
            }
        }

        private async Task AddFriend()
        {
            var userId = AvailableFriends.FirstOrDefault(u => u.DisplayName == SelectedValue)?.UserId;
            if (string.IsNullOrEmpty(userId))
            {
                SnackBar.Add("Select a user", Severity.Warning);
                return;
            }
            var currentUser = AppState.CurrentUser.UserId;
            var success = await ApiClient.AddFriend(new UserFriendModel { FirstUserId = currentUser, SecondUserId = userId });
            if (!success)
            {
                SnackBar.Add("Could not add friend", Severity.Error);
            }
            else
            {
                var addedFriend = AvailableFriends.FirstOrDefault(user => user.UserId == userId);
                FriendsList.Add(addedFriend);
                AvailableFriends = AvailableFriends.Where(user => user.UserId != userId).ToList();
                SnackBar.Add("Friend added", Severity.Success);
            }
        }
    }
}
