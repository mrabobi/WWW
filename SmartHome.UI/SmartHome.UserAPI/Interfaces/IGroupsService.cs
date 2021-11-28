using SmartHome.UserAPI.Models;
using System;
using System.Collections.Generic;

namespace SmartHome.UserAPI.Interfaces
{
    public interface IGroupsService
    {
        List<Group> GetAll();
        bool GroupExists(Guid groupId);
        Group GetById(Guid groupId);
        List<Group> GetGroupByOwner(Guid ownerId);
        Group AddGroup(Group group);
        bool DeleteGroup(Guid groupId);
        Group UpdateGroup(Group group);
        bool AddUserToGroup(Guid userId, Guid groupId);
        bool RemoveUserFromGroup(Guid userId, Guid groupId);
        bool UserIsInGroup(Guid userId, Guid groupId);
        bool AddClaimToGroup(Guid groupId, Guid claimId);
        bool RemoveClaimFromGroup(Guid groupId, Guid claimId);
        bool GroupHasClaim(Guid groupId, Guid claimId);
    }
}
