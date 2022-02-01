using System.Collections.Generic;

namespace SmartHome.Stardog.Models.Users
{
    public class GroupWithData
    {
        public Group Group { get; set; }
        public List<UserModel> GroupUsers { get; set; }
        public List<UserModel> AvailableUsers { get; set; }
        public List<ThingViewModel> GroupThings { get; set; }
        public List<ThingViewModel> AvailableThings { get; set; }

        public GroupWithData()
        {
            GroupUsers = new List<UserModel>();
            AvailableUsers = new List<UserModel>();
            GroupThings = new List<ThingViewModel>();
            AvailableThings = new List<ThingViewModel>();
        }
    }
}
