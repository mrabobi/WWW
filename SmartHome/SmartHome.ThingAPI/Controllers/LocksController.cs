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
    public class LocksController : ControllerBase
    {

        private readonly ILocksService _locksService;

        public LocksController(ILocksService locksService)
        {
            _locksService = locksService;
        }
        // GET: api/<BulbsController>
        [HttpGet]
        public ActionResult<IEnumerable<SmartLock>> Get(Guid ownerId)
        {
            return Ok(_locksService.GetLocksByOwner(ownerId));
        }

        // GET api/<BulbsController>/5
        [HttpGet("{lockId}")]
        public ActionResult<SmartBulb> Get(Guid lockId, Guid ownerId)
        {
            if (_locksService.IsLockOwner(lockId, ownerId))
            {
                return Ok(_locksService.GetById(lockId));
            }
            else
            {
                return BadRequest();
            }
        }

        // POST api/<BulbsController>
        [HttpPost]
        public IActionResult Post([FromBody] SmartLock lockModel)
        {
            if (_locksService.LockExists(lockModel.ThingId))
            {
                return BadRequest();
            }
            var lockResult = _locksService.AddLock(lockModel);
            if (lockResult == null)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Could not create lock");
            }
            return CreatedAtAction(nameof(Get), new { lockId = lockResult.ThingId, ownerId = lockModel.OwnerId }, lockResult);
        }

        [HttpPut]
        public IActionResult Put([FromBody] SmartLock lockModel)
        {
            if (_locksService.LockExists(lockModel.ThingId))
            {
                return BadRequest();
            }
            var bulbResult = _locksService.UpdateLock(lockModel);
            if (bulbResult == null)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Could not update lock");
            }
            return Ok();
        }

        // DELETE api/<BulbsController>/5
        [HttpDelete("{lockId}")]
        public IActionResult Delete(Guid lockId)
        {
            if (!_locksService.LockExists(lockId))
            {
                if (_locksService.LockExists(lockId))
                {
                    return Ok();
                }
                else
                {
                    return StatusCode(StatusCodes.Status500InternalServerError, "Could not delete lock");
                }
            }
            else
            {
                return BadRequest();
            }
        }
    }
}
