using System;

namespace SmartHome.UI.Data
{
    public class User
    {
        public Guid UserId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string GivenName { get; set; }
        public string Email { get; set; }
    }
}
