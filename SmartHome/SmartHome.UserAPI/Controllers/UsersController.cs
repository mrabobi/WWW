using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SmartHome.Stardog.Interfaces;
using SmartHome.Stardog.Models.Users;
using System;
using System.Collections.Generic;

namespace SmartHome.API.Controllers
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

        [HttpGet]
        public ActionResult<IEnumerable<UserModel>> Get()
        {
            return Ok(_userService.GetAll());
        }

        [HttpGet("{userId}")]
        public ActionResult<UserModel> Get(string userId)
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

        [HttpPost]
        public IActionResult Post([FromBody] UserModel user)
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

        // DELETE api/<BulbsController>/5
        [HttpDelete("{userId}")]
        public IActionResult Delete(string userId)
        {
            if (_userService.UserExists(userId))
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

        [HttpPost("AddFriend")]
        public IActionResult AddFriend([FromBody] UserFriendModel model)
        {
            if (_userService.CheckIfUsersAreFriends(model.FirstUserId,model.SecondUserId))
            {
                return BadRequest();
            }
            var success = _userService.AddFriend(model.FirstUserId,model.SecondUserId);
            if (!success)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Could not create user");
            }
            return Ok();
        }

        [HttpDelete("RemoveFriend")]
        public IActionResult RemoveFriend([FromBody] UserFriendModel model)
        {
            if (!_userService.CheckIfUsersAreFriends(model.FirstUserId, model.SecondUserId))
            {
                return BadRequest();
            }

            var success = _userService.RemoveFriend(model.FirstUserId, model.SecondUserId);
            if (!success)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Could not create user");
            }
            return Ok();
        }

        [HttpGet("AvailableFriends/{userId}")]
        public ActionResult<List<UserModel>> AvailableFriends(string userId)
        {
            return Ok(_userService.GetAvailableFriends(userId));
        }

        [HttpGet("GetFriends/{userId}")]
        public ActionResult<List<UserModel>> GetFriends(string userId)
        {
            return Ok(_userService.GetFriends(userId));
        }
    }
}
