using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using System.Threading.Tasks;

namespace SmartHome.UI.Pages
{
    public partial class Index
    {
        [Inject]
        public AuthenticationStateProvider GetAuthenticationStateAsync { get; set; }
        protected async override Task OnInitializedAsync()
        {
            var authstate = await GetAuthenticationStateAsync.GetAuthenticationStateAsync();
            var user = authstate.User;
            var name = user.Identity.Name;
        }
    }
}
