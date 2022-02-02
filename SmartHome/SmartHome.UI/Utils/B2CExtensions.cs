using Microsoft.AspNetCore.Authentication;
using SmartHome.UI.ApiClients;
using SmartHome.UI.Data;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace SmartHome.UI_Auth.Utils
{
    public class B2CExtensions
    {
        public static UsersApiClient ApiClient {get;set;}
        public async static Task<Task> OnTicketReceivedCallback(TicketReceivedContext context)
        {
            List<Claim> claims = context.Principal.Claims.ToList();
            bool isNewUser = claims.FirstOrDefault(x => x.Type == "newUser") == null ? false : true;
            string userId = claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;
            string firstName = claims.FirstOrDefault(c => c.Type == ClaimTypes.GivenName)?.Value;
            string lastName= claims.FirstOrDefault(c => c.Type == ClaimTypes.Surname)?.Value;
            var username = claims.FirstOrDefault(c => c.Type == "name")?.Value;
            var email = claims.FirstOrDefault(c => c.Type == "emails")?.Value;
            var user = new UserModel()
            {
                UserId = userId,
                FirstName = firstName,
                LastName = lastName,
                DisplayName = username,
                Email = email
            };

            if (isNewUser)
            {
                var success=await ApiClient.AddUser(user);
            }
            else
            {
                AppState.CurrentUser = user;
            }

            return Task.CompletedTask;
        }
    }
}
