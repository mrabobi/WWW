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
    public partial class Devices
    {
        [Inject]
        public ThingsApiClient ApiClient { get; set; }
        [Inject]
        public ISnackbar SnackBar { get; set; }
        public List<Thing> Things { get; set; }
        public bool DataIsLoading { get; set; }
        protected override async Task OnInitializedAsync()
        {
            DataIsLoading = true;
            Things = await ApiClient.GetAccesibleThings(AppState.CurrentUser.UserId);
       
            DataIsLoading = false;
        }
    }
}
