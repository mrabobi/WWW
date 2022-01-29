using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SmartHome.ThingAPI.Models
{
    public class SmartBulb : SmartThing
    {
       public bool IsOn { get; set; }
       public string Color { get; set; }
    }
}
