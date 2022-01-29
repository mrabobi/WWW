using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SmartHome.ThingAPI.Interfaces;
using SmartHome.ThingAPI.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace SmartHome.ThingAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BulbsController : ControllerBase
    {
        private readonly IBulbsService _bulbsService;

        public BulbsController(IBulbsService bulbsService)
        {
            _bulbsService = bulbsService;
        }
        // GET: api/<BulbsController>
        [HttpGet]
        public ActionResult<IEnumerable<SmartBulb>> Get(Guid ownerId)
        {
            return Ok(_bulbsService.GetBulbsByOwner(ownerId));
        }

        // GET api/<BulbsController>/5
        [HttpGet("{bulbId}")]
        public ActionResult<SmartBulb> Get(Guid bulbId, Guid ownerId)
        {
            if (_bulbsService.IsBulbOwner(bulbId, ownerId))
            {
                return Ok(_bulbsService.GetById(bulbId));
            }
            else
            {
                return BadRequest();
            }
        }

        // POST api/<BulbsController>
        [HttpPost]
        public IActionResult Post([FromBody] SmartBulb bulb)
        {
            if(_bulbsService.BulbExists(bulb.ThingId))
            {
                return BadRequest();
            }
            var bulbResult = _bulbsService.AddBulb(bulb);
            if(bulbResult==null)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Could not create bulb");
            }
            return CreatedAtAction(nameof(Get), new { bulbId = bulbResult.ThingId, ownerId = bulb.OwnerId }, bulbResult);
        }

        [HttpPut]
        public IActionResult Put([FromBody] SmartBulb bulb )
        {
            if(_bulbsService.BulbExists(bulb.ThingId))
            {
                return BadRequest();
            }
            var bulbResult = _bulbsService.UpdateBulb(bulb);
            if(bulbResult==null)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Could not update bulb");
            }
            return Ok();
        }

        // DELETE api/<BulbsController>/5
        [HttpDelete("{bulbId}")]
        public IActionResult Delete(Guid bulbId)
        {
            if(!_bulbsService.BulbExists(bulbId))
            {
                if(_bulbsService.DeleteBulb(bulbId))
                {
                    return Ok();
                }
                else
                {
                    return StatusCode(StatusCodes.Status500InternalServerError, "Could not delete bulb");
                }
            }
            else
            {
                return BadRequest();
            }
        }
    }
}
