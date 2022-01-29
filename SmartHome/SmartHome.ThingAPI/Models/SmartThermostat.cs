using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SmartHome.ThingAPI.Models
{
    public class SmartThermostat : SmartThing
    {
        public float Temperature { get; set; }
    }
}
