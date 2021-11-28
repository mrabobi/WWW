using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SmartHome.ThingAPI.Interfaces;
using SmartHome.ThingAPI.Models;
using System;
using System.Collections.Generic;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace SmartHome.ThingAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ThermostatsController : ControllerBase
    {

        private readonly IThermostatsService _thermostatsService;

        public ThermostatsController(IThermostatsService thermostatsService)
        {
            _thermostatsService = thermostatsService;
        }
        // GET: api/<BulbsController>
        [HttpGet]
        public ActionResult<IEnumerable<SmartLock>> Get(Guid ownerId)
        {
            return Ok(_thermostatsService.GetThermostatsByOwner(ownerId));
        }

        // GET api/<BulbsController>/5
        [HttpGet("{thermostatId}")]
        public ActionResult<SmartBulb> Get(Guid thermostatId, Guid ownerId)
        {
            if (_thermostatsService.IsThermostatOwner(thermostatId, ownerId))
            {
                return Ok(_thermostatsService.GetById(thermostatId));
            }
            else
            {
                return BadRequest();
            }
        }

        // POST api/<BulbsController>
        [HttpPost]
        public IActionResult Post([FromBody] SmartThermostat thermostat)
        {
            if (_thermostatsService.ThermostatExists(thermostat.ThingId))
            {
                return BadRequest();
            }
            var thermostatResult = _thermostatsService.AddThermostat(thermostat);
            if (thermostatResult == null)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Could not create thermostat");
            }
            return CreatedAtAction(nameof(Get), new { thermostatId = thermostatResult.ThingId, ownerId = thermostat.OwnerId }, thermostatResult);
        }

        [HttpPut]
        public IActionResult Put([FromBody] SmartThermostat thermostat)
        {
            if (_thermostatsService.ThermostatExists(thermostat.ThingId))
            {
                return BadRequest();
            }
            var bulbResult = _thermostatsService.UpdateThermostat(thermostat);
            if (bulbResult == null)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Could not update thermostat");
            }
            return Ok();
        }

        // DELETE api/<BulbsController>/5
        [HttpDelete("{thermostatId}")]
        public IActionResult Delete(Guid thermostatId)
        {
            if (!_thermostatsService.ThermostatExists(thermostatId))
            {
                if (_thermostatsService.ThermostatExists(thermostatId))
                {
                    return Ok();
                }
                else
                {
                    return StatusCode(StatusCodes.Status500InternalServerError, "Could not delete thermostat");
                }
            }
            else
            {
                return BadRequest();
            }
        }
    }
}

