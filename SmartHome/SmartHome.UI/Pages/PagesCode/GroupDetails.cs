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
    public partial class GroupDetails
    {
        [Inject]
        public UsersApiClient ApiClient { get; set; }
        [Inject]
        public ISnackbar SnackBar { get; set; }
        [Inject]
        public NavigationManager NavigationManager { get; set; }
        public GroupWithData GroupData { get; set; }
        public bool DataIsLoading { get; set; }
        private string searchStringUsers = "";
        private string searchStringThings = "";
        public List<string> AvaiableUsersNames => GroupData.AvailableUsers.Select(u => u.DisplayName).ToList();
        public List<string> AvaiableThingNames => GroupData.AvailableThings.Select(u =>u.thingId).ToList();
        public string SelectedValueUsers { get; set; }

        public string SelectedValueThings { get; set; }

        protected override async Task OnInitializedAsync()
        {
            DataIsLoading = true;
            GroupData = await ApiClient.GetGroupWithUsers(AppState.CurrentGroupId);
            DataIsLoading = false;
        }

        private async Task<IEnumerable<string>> SearchUsers(string value)
        {
            await Task.Delay(5);

            if (string.IsNullOrEmpty(value))
            {
                return AvaiableUsersNames;
            }
            return AvaiableUsersNames.Where(x => x.Contains(value, StringComparison.InvariantCultureIgnoreCase));
        }

        private async Task<IEnumerable<string>> SearchThings(string value)
        {
            await Task.Delay(5);

            if (string.IsNullOrEmpty(value))
            {
                return AvaiableThingNames;
            }
            return AvaiableThingNames.Where(x => x.Contains(value, StringComparison.InvariantCultureIgnoreCase));
        }


        private bool FilterFuncUsers1(UserModel element) => FilterFuncUsers(element, searchStringUsers);

        private bool FilterFuncUsers(UserModel element, string searchString)
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

        private bool FilterFuncThings1(ThingViewModel element) => FilterFuncThings(element, searchStringUsers);

        private bool FilterFuncThings(ThingViewModel element, string searchString)
        {
            if (string.IsNullOrWhiteSpace(searchString))
            {
                return true;
            }

            if ($"{element.description} {element.title}".Contains(searchString))
            {
                return true;
            }
            return false;
        }

        private async Task RemoveUserFromGroup(string userId)
        {
            var success = await ApiClient.RemoveUserFromGroup(new UserGroupModel { UserId = userId, GroupId = GroupData.Group.GroupId });
            if (!success)
            {
                SnackBar.Add("Could not remove user", Severity.Error);
            }
            else
            {
                var deletedFriend = GroupData.GroupUsers.FirstOrDefault(user => user.UserId == userId);
                GroupData.AvailableUsers.Add(deletedFriend);
                GroupData.GroupUsers = GroupData.GroupUsers.Where(user => user.UserId != userId).ToList();
                SnackBar.Add("User removed", Severity.Success);
            }
        }

        private async Task AddUserToGroup()
        {
            var userId = GroupData.AvailableUsers.FirstOrDefault(u => u.DisplayName == SelectedValueUsers)?.UserId;
            if (string.IsNullOrEmpty(userId))
            {
                SnackBar.Add("Select a user", Severity.Warning);
                return;
            }
            var success = await ApiClient.AddUserToGroup(new UserGroupModel { UserId = userId, GroupId = GroupData.Group.GroupId });
            if (!success)
            {
                SnackBar.Add("Could not add user to group", Severity.Error);
            }
            else
            {
                var addedUser = GroupData.AvailableUsers.FirstOrDefault(user => user.UserId == userId);
                GroupData.GroupUsers.Add(addedUser);
                GroupData.AvailableUsers = GroupData.AvailableUsers.Where(user => user.UserId != userId).ToList();
                SnackBar.Add("User added", Severity.Success);
            }
        }

        private async Task RemoveThingClaim(string thingId)
        {
            var currentGroup = AppState.CurrentGroupId;
            var success = await ApiClient.RemoveClaim(new GroupResourceModel { GroupId = currentGroup, Claim = thingId });
            if (!success)
            {
                SnackBar.Add("Could not remove claim", Severity.Error);
            }
            else
            {
                var deletedClaim = GroupData.GroupThings.FirstOrDefault(thing => thing.thingId == thingId);
                GroupData.AvailableThings.Add(deletedClaim);
                GroupData.GroupThings = GroupData.GroupThings.Where(thing => thing.thingId != thingId).ToList();
                SnackBar.Add("Claim removed", Severity.Success);
            }
        }

        private async Task AddThingClaimToGroup()
        {
            var thingId = GroupData.AvailableThings.FirstOrDefault(t =>t.thingId  == SelectedValueThings)?.thingId;
            var currrentGroup = AppState.CurrentGroupId;
            if (string.IsNullOrEmpty(thingId))
            {
                SnackBar.Add("Select a claim", Severity.Warning);
                return;
            }
            var currentUser = AppState.CurrentUser.UserId;
            var success = await ApiClient.AddClaim(new GroupResourceModel { GroupId = currrentGroup, Claim = thingId });
            if (!success)
            {
                SnackBar.Add("Could not add claim to group", Severity.Error);
            }
            else
            {
                var addedClaim = GroupData.AvailableThings.FirstOrDefault(thing => thing.thingId == thingId);
                GroupData.GroupThings.Add(addedClaim);
                GroupData.AvailableThings = GroupData.AvailableThings.Where(thing => thing.thingId != thingId).ToList();
                SnackBar.Add("Claim added", Severity.Success);
            }
        }

    }
}
