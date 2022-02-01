using Microsoft.AspNetCore.Mvc;
using SmartHome.Stardog.Models;
using SmartHome.Stardog.Services;
using System.Collections.Generic;

namespace SmartHome.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ThingsController : ControllerBase
    {
        private readonly ThingsService _thingsService;
        public ThingsController(ThingsService thingsService)
        {
            _thingsService = thingsService;
        }
        [HttpGet]
        public ActionResult<IEnumerable<Thing>> Get()
        {
            return Ok(_thingsService.GetAll());
        }

        [HttpPost]
        public ActionResult<Thing> Post(Thing thing)
        {
            return Ok(_thingsService.AddThing(thing));
        }

        [HttpGet("GetByOwner/{ownerId}")]
        public ActionResult<Thing> GetByOwner(string ownerId)
        {
            return Ok(_thingsService.GetByOwner(ownerId));
        }

        [HttpGet("GetAccessible/{userId}")]
        public ActionResult<Thing> GetAccessible(string userId)
        {
            return Ok(_thingsService.GetAccesible(userId));
        }

    }
}
