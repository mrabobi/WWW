using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SmartHome.UserAPI.Models
{
    public class GroupResourceModel
    {
        public Guid groupId { get; set; }
        public Guid otherEntityId { get; set; }
    }
}
