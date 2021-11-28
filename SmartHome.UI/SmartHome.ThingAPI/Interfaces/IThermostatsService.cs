using SmartHome.ThingAPI.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SmartHome.ThingAPI.Interfaces
{
    public interface IThermostatsService
    {
        public SmartThermostat AddThermostat(SmartThermostat thermostat);
        public bool DeleteThermostat(Guid thermostatId);
        public SmartThermostat UpdateThermostat(SmartThermostat thermostat);
        public List<SmartThermostat> GetThermostatsByOwner(Guid ownerId);
        public bool IsThermostatOwner(Guid thermostatId, Guid ownerId);
        public SmartThermostat GetById(Guid thermostatId);
        public bool ThermostatExists(Guid thermostatId);
    }
}
