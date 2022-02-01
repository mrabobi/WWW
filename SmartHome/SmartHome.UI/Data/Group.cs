using System.Collections.Generic;

namespace SmartHome.UI.Data
{
    public class Group
    {
        public string GroupId { get; set; }
        public string OwnerId { get; set; }
        public string GroupName { get; set; }
        public List<string> Claims { get; set; }
    }
}
