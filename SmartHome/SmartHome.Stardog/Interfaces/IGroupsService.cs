using SmartHome.Stardog.Models;
using SmartHome.Stardog.Models.Users;
using System;
using System.Collections.Generic;

namespace SmartHome.Stardog.Interfaces
{
    public interface IGroupsService
    {
        List<Group> GetAll();
        bool GroupExists(string groupName,string userId);
        bool GroupExists(string groupId);
        Group GetById(string groupId);
        List<Group> GetGroupsByOwner(string ownerId);
        Group AddGroup(Group group);
        bool DeleteGroup(string groupId);
        bool AddUserToGroup(string userId, string groupId);
        bool RemoveUserFromGroup(string userId, string groupId);
        bool UserIsInGroup(string userId, string groupId);
        bool AddClaimToGroup(string groupId, string claim);
        bool RemoveClaimFromGroup(string groupId, string claim);
        bool GroupHasClaim(string groupId, string claim);
        public List<string> GetUsersInGroup(string groupId);
        public List<string> GetAccessibleDevices(string groupId);
        public List<string> GetAvailableUsersForGroup(string groupId);
    }
}
