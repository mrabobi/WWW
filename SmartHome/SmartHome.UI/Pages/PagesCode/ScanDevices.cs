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
    public partial class ScanDevices
    {
        [Inject]
        public ThingsApiClient ApiClient { get; set; }
        [Inject]
        public ISnackbar SnackBar { get; set; }
        public List<ThingViewModel> Things { get; set; }
        public bool DataIsLoading { get; set; }

        protected override async Task OnInitializedAsync()
        {
            DataIsLoading = true;
            Things = await ApiClient.ScanThings(AppState.DeviceApiUrl);
            DataIsLoading = false;
        }

        public async Task ActivateThing(ThingViewModel thing)
        {
            DataIsLoading = true;
            var success = await ApiClient.ActivateThing(thing);
            if(success)
            {
                Things=Things.Where(t => t.validation_url != thing.validation_url).ToList();
                SnackBar.Add("Device activated", Severity.Success);
            }
            else
            {
                SnackBar.Add("Could not activate device, check the activation key", Severity.Error);
            }
            DataIsLoading = false;
        }
    }
}
