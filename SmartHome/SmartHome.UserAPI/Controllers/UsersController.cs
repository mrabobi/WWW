using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SmartHome.UserAPI.Interfaces;
using SmartHome.UserAPI.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace SmartHome.UserAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly IUserService _userService;

        public UsersController(IUserService userService)
        {
            _userService = userService;
        }
        // GET: api/<BulbsController>
        [HttpGet]
        public ActionResult<IEnumerable<User>> Get()
        {
            return Ok(_userService.GetAll());
        }

        // GET api/<BulbsController>/5
        [HttpGet("{userId}")]
        public ActionResult<User> Get(Guid userId)
        {
            if (_userService.UserExists(userId))
            {
                return Ok(_userService.GetById(userId));
            }
            else
            {
                return BadRequest();
            }
        }

        // POST api/<BulbsController>
        [HttpPost]
        public IActionResult Post([FromBody] User user)
        {
            if (_userService.UserExists(user.UserId))
            {
                return BadRequest();
            }
            var userResult = _userService.AddUser(user);
            if (userResult == null)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Could not create user");
            }
            return CreatedAtAction(nameof(Get), new { userId = userResult.UserId}, userResult);
        }

        [HttpPut]
        public IActionResult Put([FromBody] User user)
        {
            if (!_userService.UserExists(user.UserId))
            {
                return BadRequest();
            }
            var bulbResult = _userService.UpdateUser(user);
            if (bulbResult == null)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Could not update user");
            }
            return Ok();
        }

        // DELETE api/<BulbsController>/5
        [HttpDelete("{userId}")]
        public IActionResult Delete(Guid userId)
        {
            if (!_userService.UserExists(userId))
            {
                if (_userService.DeleteUser(userId))
                {
                    return Ok();
                }
                else
                {
                    return StatusCode(StatusCodes.Status500InternalServerError, "Could not delete user");
                }
            }
            else
            {
                return BadRequest();
            }
        }
    }
}
