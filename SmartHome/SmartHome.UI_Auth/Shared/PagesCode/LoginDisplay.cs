
using Microsoft.AspNetCore.Components;

namespace SmartHome.UI.Shared
{
    public partial class LoginDisplay
    {
        [Inject]
        public NavigationManager NavigationManager { get; set; }

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
