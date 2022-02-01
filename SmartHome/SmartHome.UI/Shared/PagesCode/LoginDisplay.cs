using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using SmartHome.UI.Data;
using SmartHome.UI_Auth.Utils;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace SmartHome.UI.Shared
{
    public partial class LoginDisplay
    {
        [Inject]
        public NavigationManager NavigationManager { get; set; }

        [Inject]
        public AuthenticationStateProvider GetAuthenticationStateAsync { get; set; }

        protected async override Task OnInitializedAsync()
        {
            if (AppState.CurrentUser == null)
            {
                var authstate = await GetAuthenticationStateAsync.GetAuthenticationStateAsync();
                var user = authstate.User;
                if(user!=null)
                {
                    List<Claim> claims = user.Claims.ToList();
                    bool isNewUser = claims.FirstOrDefault(x => x.Type == "newUser") == null ? false : true;
                    string userId = claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;
                    string firstName = claims.FirstOrDefault(c => c.Type == ClaimTypes.GivenName)?.Value;
                    string lastName = claims.FirstOrDefault(c => c.Type == ClaimTypes.Surname)?.Value;
                    var username = claims.FirstOrDefault(c => c.Type == "name")?.Value;
                    var email = claims.FirstOrDefault(c => c.Type == "emails")?.Value;
                    var currentUser = new UserModel()
                    {
                        UserId = userId,
                        FirstName = firstName,
                        LastName = lastName,
                        DisplayName = username,
                        Email = email
                    };
                    AppState.CurrentUser = currentUser;
                }
            }
        }
        public void RedirectToSignIn()
        {
            NavigationManager.NavigateTo("MicrosoftIdentity/Account/SignIn");
        }

        public void RedirectToSignOut()
        {
            NavigationManager.NavigateTo("MicrosoftIdentity/Account/SignOut");
        }
        public void RedirectToHome()
        {
            NavigationManager.NavigateTo("");
        }

    }
}
