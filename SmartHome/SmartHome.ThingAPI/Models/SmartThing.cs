using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SmartHome.ThingAPI.Models
{
    public class SmartThing
    {
        public Guid ThingId { get; set; }
        public Guid OwnerId { get; set; }
        public string Room { get; set; }
    }
}
