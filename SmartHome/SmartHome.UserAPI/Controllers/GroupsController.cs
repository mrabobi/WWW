using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SmartHome.Stardog.Interfaces;
using SmartHome.Stardog.Models;
using SmartHome.Stardog.Models.Users;
using SmartHome.Stardog.Services;
using System.Collections.Generic;
using System.Linq;

namespace SmartHome.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class GroupsController : ControllerBase
    {
        private readonly IGroupsService _groupsService;
        private readonly IUserService _userService;
        private readonly ThingsService _thingsService;

        public GroupsController(IGroupsService groupsService,IUserService userService,ThingsService thingsService)
        {
            _groupsService = groupsService;
            _userService = userService;
            _thingsService = thingsService;
        }

        [HttpGet]
        public ActionResult<IEnumerable<Group>> Get()
        {
            return Ok(_groupsService.GetAll());
        }

        [HttpGet("GetGroupsByOnwer/{ownerId}")]
        public ActionResult<IEnumerable<Group>> GetGroupsByOnwer(string ownerId)
        {
            return Ok(_groupsService.GetGroupsByOwner(ownerId));
        }

        [HttpGet("GetGroupsByOnwerWithUsers/{ownerId}")]
        public ActionResult<IEnumerable<GroupWithData>> GetGroupsByOnwerWithUsers(string ownerId)
        {
            var groupsWithUsers = new List<GroupWithData>();
            var groups = _groupsService.GetGroupsByOwner(ownerId);
            foreach(var group in groups)
            {
                var groupUsers = _groupsService.GetUsersInGroup(group.GroupId).Select(u=>_userService.GetById(u)).ToList();
                var availalbeUsers = _groupsService.GetAvailableUsersForGroup(group.GroupId).Select(u => _userService.GetById(u)).ToList();
                var groupWithUsers = new GroupWithData()
                {
                    Group = group,
                    GroupUsers = groupUsers,
                    AvailableUsers = availalbeUsers
                };
                groupsWithUsers.Add(groupWithUsers);
            }
            return Ok(groupsWithUsers);
        }

        [HttpGet("{groupdId}")]
        public ActionResult<Group> Get(string groupdId)
        {
            if (_groupsService.GroupExists(groupdId))
            {
                return Ok(_groupsService.GetById(groupdId));
            }
            else
            {
                return BadRequest();
            }
        }
        
        [HttpGet("GetWithUsers/{groupdId}")]
        public ActionResult<Group> GetWithUsers(string groupdId)
        {
            if (_groupsService.GroupExists(groupdId))
            {
                var group = _groupsService.GetById(groupdId);
                var groupUsers = _groupsService.GetUsersInGroup(groupdId).Select(u => _userService.GetById(u)).ToList();
                var availableUsers = _groupsService.GetAvailableUsersForGroup(groupdId).Select(u => _userService.GetById(u)).ToList();
                var availableThings = _thingsService.GetForGroup(groupdId, false);
                var groupThings = _thingsService.GetForGroup(groupdId, true);
                var groupWithUsers = new GroupWithData()
                {
                    Group = group,
                    GroupUsers = groupUsers,
                    AvailableUsers = availableUsers,
                    AvailableThings=availableThings,
                    GroupThings=groupThings
                };
                return Ok(groupWithUsers);
            }
            else
            {
                return BadRequest();
            }
        }

        [HttpPost]
        public IActionResult Post([FromBody] Group group)
        {
            if (_groupsService.GroupExists(group.GroupName,group.OwnerId))
            {
                return BadRequest();
            }
            var groupResult = _groupsService.AddGroup(group);
            if (groupResult == null)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Could not create group");
            }
            return CreatedAtAction(nameof(Get), new { groupId = groupResult.GroupId }, groupResult);
        }

        [HttpDelete("{groupdId}")]
        public IActionResult Delete(string groupdId)
        {
            if (_groupsService.GroupExists(groupdId))
            {
                if (_groupsService.DeleteGroup(groupdId))
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

        [HttpPost("AddUserToGroup")]
        public IActionResult AddUserToGroup([FromBody] UserGroupModel model)
        {
            if (_groupsService.UserIsInGroup(model.GroupId,model.UserId))
            {
                return BadRequest();
            }
            var success = _groupsService.AddUserToGroup(model.UserId,model.GroupId);
            if (!success)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Could add user to group");
            }
            return Ok();
        }

        [HttpDelete("RemoveUserFromGroup")]
        public IActionResult RemoveUserFromGroup([FromBody] UserGroupModel model)
        {
            if (!_groupsService.UserIsInGroup(model.GroupId,model.UserId))
            {
                return BadRequest();
            }
            var success = _groupsService.RemoveUserFromGroup(model.UserId, model.GroupId);
            if (!success)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Could not remove user from group");
            }
            return Ok();
        }

        [HttpPost("AddClaimToGroup")]
        public IActionResult AddClaimToGroup([FromBody] GroupResourceModel model)
        {
            if (_groupsService.GroupHasClaim(model.GroupId, model.Claim))
            {
                return BadRequest();
            }
            var success = _groupsService.AddClaimToGroup(model.GroupId, model.Claim);
            if (!success)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Could add claim to group");
            }
            return Ok();
        }

        [HttpDelete("RemoveClaimFromGroup")]
        public IActionResult RemoveClaimFromGroup([FromBody] GroupResourceModel model)
        {
            if (!_groupsService.GroupHasClaim(model.GroupId, model.Claim))
            {
                return BadRequest();
            }
            var success = _groupsService.RemoveClaimFromGroup(model.GroupId, model.Claim);
            if (!success)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Could not remove claim from group");
            }
            return Ok();
        }

        [HttpGet("UsersInGroup/{groupId}")]
        public ActionResult<List<UserModel>> GetUsersInGroup(string groupId)
        {
            var userIds = _groupsService.GetUsersInGroup(groupId);
            var users = new List<UserModel>();
            foreach (var userId in userIds)
            {
                users.Add(_userService.GetById(userId));
            }
            return Ok(users);
        }

        [HttpGet("AvailabeUsers/{groupId}")]
        public ActionResult<List<UserModel>> GetAvailableUsersForGroup(string groupId)
        {
            var userIds = _groupsService.GetAvailableUsersForGroup(groupId);
            var users = new List<UserModel>();
            foreach(var userId in userIds)
            {
                users.Add(_userService.GetById(userId));
            }
            return Ok(users);
        }
    }
}
