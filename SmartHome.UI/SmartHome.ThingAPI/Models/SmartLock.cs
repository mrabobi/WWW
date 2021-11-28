using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SmartHome.ThingAPI.Models
{
    public class SmartLock : SmartThing
    {
        public bool IsLocked { get; set; }
    }
}
