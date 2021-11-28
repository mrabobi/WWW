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
    public class GroupsController : ControllerBase
    {
        private readonly IGroupsService _groupsService;

        public GroupsController(IGroupsService userService)
        {
            _groupsService = userService;
        }

        [HttpGet]
        public ActionResult<IEnumerable<Group>> Get()
        {
            return Ok(_groupsService.GetAll());
        }

        // GET: api/<BulbsController>
        [HttpGet("GetGroupsByOnwer")]
        public ActionResult<IEnumerable<Group>> GetGroupsByOnwer(Guid ownerId)
        {
            return Ok(_groupsService.GetGroupByOwner(ownerId));
        }

        // GET api/<BulbsController>/5
        [HttpGet("{groupdId}")]
        public ActionResult<User> Get(Guid groupdId)
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

        // POST api/<BulbsController>
        [HttpPost]
        public IActionResult Post([FromBody] Group group)
        {
            if (_groupsService.GroupExists(group.GroupId))
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

        [HttpPut]
        public IActionResult Put([FromBody] Group group)
        {
            if (_groupsService.GroupExists(group.GroupId))
            {
                return BadRequest();
            }
            var groupResult = _groupsService.UpdateGroup(group);
            if (groupResult == null)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Could not update group");
            }
            return Ok();
        }

        // DELETE api/<BulbsController>/5
        [HttpDelete("{groupdId}")]
        public IActionResult Delete(Guid groupdId)
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
        public IActionResult AddUserToGroup([FromBody] GroupResourceModel model)
        {
            if (_groupsService.UserIsInGroup(model.groupId,model.otherEntityId))
            {
                return BadRequest();
            }
            var success = _groupsService.AddUserToGroup(model.groupId,model.otherEntityId);
            if (!success)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Could add user to group");
            }
            return Ok();
        }

        [HttpPost("RemoveUserFromGroup")]
        public IActionResult RemoveUserFromGroup([FromBody] GroupResourceModel model)
        {
            if (!_groupsService.UserIsInGroup(model.groupId,model.otherEntityId))
            {
                return BadRequest();
            }
            var success = _groupsService.RemoveUserFromGroup(model.groupId,model.otherEntityId);
            if (!success)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Could not remove user from group");
            }
            return Ok();
        }

        [HttpPost("AddClaimToGroup")]
        public IActionResult AddClaimToGroup([FromBody] GroupResourceModel model)
        {
            if (_groupsService.GroupHasClaim(model.groupId, model.otherEntityId))
            {
                return BadRequest();
            }
            var success = _groupsService.AddClaimToGroup(model.groupId, model.otherEntityId);
            if (!success)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Could add claim to group");
            }
            return Ok();
        }

        [HttpPost("RemoveClaimFromGroup")]
        public IActionResult RemoveClaimFromGroup([FromBody] GroupResourceModel model)
        {
            if (!_groupsService.GroupHasClaim(model.groupId, model.otherEntityId))
            {
                return BadRequest();
            }
            var success = _groupsService.RemoveClaimFromGroup(model.groupId, model.otherEntityId);
            if (!success)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Could not remove claim from group");
            }
            return Ok();
        }
    }
}
