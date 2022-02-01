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

        [HttpGet("ScanThings")]
        public ActionResult<IEnumerable<ThingViewModel>> ScanThings()
        {
            var result = new List<ThingViewModel>()
            { new ThingViewModel()
            {
                title="LightBuld",
                description="A great lightbub",
                validation_url="https://localhost:44310/api/Things/ActivateThing"
            },
            new ThingViewModel()
            {
                title="Lock",
                description="A great lock",
                validation_url="https://localhost:44310/api/Things/ActivateThing"
            }
            };
            return result;
        }

        [HttpPost("ActivateThing")]
        public ActionResult<IEnumerable<ThingViewModel>> ActivateThing([FromBody] ValueObject value)
        {
            return BadRequest();
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
    public class ValueObject
    {
        public string value { get; set; }
    }
}
