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
    public partial class CreateGroup
    {
        [Inject]
        public UsersApiClient ApiClient { get; set; }
        [Inject]
        public ISnackbar SnackBar { get; set; }
        public string Name { get; set; }
        public async Task AddGroup()
        {
            if(string.IsNullOrEmpty(Name))
            {
                SnackBar.Add("Enter group name", Severity.Warning);
                return;
            }
            var group = new Group
            {
                GroupName = Name,
                OwnerId = AppState.CurrentUser.UserId,
                Claims = new List<string>()
            };
            var success = await ApiClient.AddGroup(group);
            if(success)
            {
                SnackBar.Add("Group added", Severity.Success);
            }
            else
            {
                SnackBar.Add("Could not add group", Severity.Error);
            }

        }
    }
}
