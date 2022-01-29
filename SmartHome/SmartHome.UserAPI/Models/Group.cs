using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SmartHome.UserAPI.Models
{
    public class Group
    {
        public Guid GroupId { get; set; }
        public Guid OwnerId { get; set; }
        public string GroupName { get; set; }
        public List<Claim> Claims { get; set; }
    }
}
