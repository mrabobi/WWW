using Microsoft.AspNetCore.Components;
using MudBlazor;
using SmartHome.UI.ApiClients;
using SmartHome.UI.Data;
using SmartHome.UI_Auth.Utils;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SmartHome.UI.Pages
{
    public partial class Groups
    {
        [Inject]
        public UsersApiClient ApiClient { get; set; }
        [Inject]
        public ISnackbar SnackBar { get; set; }
        [Inject]
        public NavigationManager NavigationManager { get; set; }
        public List<Group> UserGroups { get; set; }
        public bool DataIsLoading { get; set; }
        private string searchString1 = "";

        protected override async Task OnInitializedAsync()
        {
            DataIsLoading = true;
            UserGroups = await ApiClient.GetGroups(AppState.CurrentUser.UserId);
            DataIsLoading = false;
        }

        private bool FilterFunc1(Group element) => FilterFunc(element, searchString1);

        private bool FilterFunc(Group element, string searchString)
        {
            if (string.IsNullOrWhiteSpace(searchString))
            {
                return true;
            }

            if (element.GroupName.Contains(searchString))
            {
                return true;
            }
            return false;
        }

        private async Task RemoveGroup(string groupId)
        {
            var currentUser = AppState.CurrentUser.UserId;
            var success = await ApiClient.RemoveGroup(groupId);
            if (!success)
            {
                SnackBar.Add("Could not delete group", Severity.Error);
            }
            else
            {
                UserGroups = UserGroups.Where(g => g.GroupId != groupId).ToList();
                SnackBar.Add("Group deleted", Severity.Success);
            }
        }

        private  void GoToDetails(string groupId)
        {
            AppState.CurrentGroupId = groupId;
            NavigationManager.NavigateTo("GroupDetails", forceLoad: true);
        }
    }
}
