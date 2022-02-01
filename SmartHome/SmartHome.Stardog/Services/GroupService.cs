using Serilog;
using SmartHome.Stardog.Interfaces;
using SmartHome.Stardog.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using VDS.RDF.Query;

namespace SmartHome.Stardog.Services
{
    public class GroupService : StardogService, IGroupsService
    {
        public GroupService(StardogData data, ILogger logger) : base(data, logger)
        {

        }

        public bool AddClaimToGroup(string groupId, string claim)
        {
            try
            {
                var connector = GetStardogConnector();
                var query = $"INSERT DATA {{<{GetGroupObjectUrl(_data.BaseObjectUrl, groupId)}> foaf:topic_interest <{claim}>}}";
                connector.Update(query);
                return true;
            }
            catch (Exception e)
            {
                _logger.Error("Could not add thing to Group", e);
                return false;
            }
        }

        public Group AddGroup(Group group)
        {
            try
            {
                group.GroupId = Guid.NewGuid().ToString();
                var connector = GetStardogConnector();
                var query = $"INSERT DATA {{<{GetGroupObjectUrl(_data.BaseObjectUrl, group.GroupId)}> a foaf:Group; " +
                    $"foaf:maker <{GetUserObjectUrl(_data.BaseObjectUrl, group.OwnerId)}>; " +
                    $"foaf:name '{group.GroupName}';" +
                    $"foaf:openid '{group.GroupId}'}}";
                connector.Update(query);
                return group;
            }
            catch (Exception e)
            {
                _logger.Error("Could not add Group", e);
                return null;
            }
        }

        public bool AddUserToGroup(string userId, string groupId)
        {
            try
            {
                var connector = GetStardogConnector();
                var query = $"INSERT DATA {{<{GetUserObjectUrl(_data.BaseObjectUrl, userId)}> foaf:member <{GetGroupObjectUrl(_data.BaseObjectUrl, groupId)}>}}";
                connector.Update(query);
                return true;
            }
            catch (Exception e)
            {
                _logger.Error("Could not add user to Group", e);
                return false;
            }
        }

        public bool DeleteGroup(string groupId)
        {
            try
            {
                var group = GetById(groupId);
                var users = GetUsersInGroup(groupId);
                var things = GetAccessibleDevices(groupId);
                var connector = GetStardogConnector();
                var query = $"DELETE DATA {{<{GetGroupObjectUrl(_data.BaseObjectUrl, group.GroupId)}> a foaf:Group; " +
                    $"foaf:maker <{GetUserObjectUrl(_data.BaseObjectUrl, group.OwnerId)}>; " +
                    $"foaf:name '{group.GroupName}';" +
                    $"foaf:openid '{group.GroupId}'}}";
                connector.Update(query);
                foreach(var user in users)
                {
                    query = $"DELETE DATA {{<{GetUserObjectUrl(_data.BaseObjectUrl, user)}> foaf:member <{GetGroupObjectUrl(_data.BaseObjectUrl, groupId)}>}}";
                    connector.Update(query);
                }
                foreach(var thing in things)
                {
                    query = $"DELETE DATA {{<{GetGroupObjectUrl(_data.BaseObjectUrl, groupId)}> foaf:topic_interest <{thing}>}}";
                    connector.Update(query);
                }
                return true;
            }
            catch (Exception e)
            {
                _logger.Error("Could not delete Group", e);
                return false;
            }
        }

        public List<string> GetAccessibleDevices(string groupId)
        {
            try
            {
                var connector = GetStardogConnector();
                var query = $"SELECT DISTINCT ?thing  {{<{GetGroupObjectUrl(_data.BaseObjectUrl, groupId)}> foaf:topic_interest ?thing}}";
                var results = (SparqlResultSet)connector.Query(query);
                var devices = new List<string>();
                foreach (var result in results)
                {
                    if (result["thing"] != null)
                    {
                        devices.Add(result["thing"].ToString());
                    }
                }
                return devices;
            }
            catch (Exception e)
            {
                _logger.Error("Could not retrieve devices for group", e);
                return new List<string>();
            }
        }

        public List<Group> GetAll()
        {
            try
            {
                var connector = GetStardogConnector();
                var query = $"SELECT DISTINCT *  {{?group a foaf:Group; " +
                    $"foaf:name ?name; " +
                    $"foaf:openid ?openid;" +
                    $"foaf:maker ?owner}}";
                var result = (SparqlResultSet)connector.Query(query);
                var groups = AssembleQueryResult(result);
                foreach(var group in groups)
                {
                    group.Claims = GetAccessibleDevices(group.GroupId);
                }
                return groups;
              
            }
            catch (Exception e)
            {
                _logger.Error($"Could not retrieve groups", e);
                return null;
            }
        }

        //public List<string> GetAvailableUsersForGroup(string groupId)
        //{
        //    try {
        //        var inGroup = GetUsersInGroup(groupId);
        //        var group = GetById(groupId);
        //        var all = GetUsers();
        //        var available = all.Where(u => inGroup.All(u2 => u2 != u) && u != group.OwnerId).ToList();
        //        return available;
        //    }
        //    catch (Exception e)
        //    {
        //        _logger.Error("Could not retrieve available users for group", e);
        //        return new List<string>();
        //    }
        //}

        public List<string> GetAvailableUsersForGroup(string groupId)
        {
            try
            {
                var connector = GetStardogConnector();
                var query = $"SELECT DISTINCT ?userId WHERE {{?user a foaf:Person; foaf:openid ?userId " +
                    $"FILTER(NOT EXISTS {{?user foaf:member <{GetGroupObjectUrl(_data.BaseObjectUrl, groupId)}>}}" +
                    $" && NOT EXISTS{{<{GetGroupObjectUrl(_data.BaseObjectUrl, groupId)}> foaf:maker ?user}}" +
                    $"&& EXISTS{{<{GetGroupObjectUrl(_data.BaseObjectUrl, groupId)}> foaf:maker ?ownerUser. ?ownerUser foaf:knows ?user }})}}";
                var results = (SparqlResultSet)connector.Query(query);
                var users = new List<string>();
                foreach (var result in results)
                {
                    if (result["userId"] != null)
                    {
                        users.Add(result["userId"].ToString());
                    }
                }
                return users;
            }
            catch (Exception e)
            {
                _logger.Error("Could not retrieve available users for group", e);
                return new List<string>();
            }
        }

        public Group GetById(string groupId)
        {
            try
            {
                var connector = GetStardogConnector();
                var query = $"SELECT DISTINCT *  {{?group a foaf:Group; " +
                    $"foaf:name ?name; " +
                    $"foaf:openid ?openid; " +
                    $"foaf:maker ?owner. " +
                    $"FILTER(STR(?openid) = '{groupId}')}}";
                var result = (SparqlResultSet)connector.Query(query);
                var group=AssembleQueryResult(result).FirstOrDefault();
                if(group!=null)
                {
                    group.Claims = GetAccessibleDevices(groupId);
                }
                return group;
            }
            catch (Exception e)
            {
                _logger.Error($"Could not retrieve group with id {groupId}", e);
                return null;
            }
        }

        public List<Group> GetGroupsByOwner(string ownerId)
        {
            try
            {
                var connector = GetStardogConnector();
                var query = $"SELECT DISTINCT ?groupId  {{ ?group a foaf:Group;" +
                    $" foaf:maker <{GetUserObjectUrl(_data.BaseObjectUrl, ownerId)}>;" +
                                                    $"foaf:openid ?groupId}}";
                var results = (SparqlResultSet)connector.Query(query);
                var groups = new List<string>();
                foreach (var result in results)
                {
                    if (result["groupId"] != null)
                    {
                        groups.Add(result["groupId"].ToString());
                    }
                }
                return groups.Select(g=>GetById(g)).ToList();
            }
            catch (Exception e)
            {
                _logger.Error("Could not retrieve groups for owner", e);
                return new List<Group>();
            }
        }

        public List<string> GetUsersInGroup(string groupId)
        {
            try
            {
                var connector = GetStardogConnector();
                var query = $"SELECT DISTINCT ?user  {{ ?user foaf:member <{GetGroupObjectUrl(_data.BaseObjectUrl, groupId)}>}}";
                var results = (SparqlResultSet)connector.Query(query);
                var users = new List<string>();
                foreach (var result in results)
                {
                    if (result["user"] != null)
                    {
                        var userId= result["user"]?.ToString().Replace($"{_data.BaseObjectUrl}/Users/", "");
                        users.Add(userId);
                    }
                }
                return users;
            }
            catch (Exception e)
            {
                _logger.Error("Could not retrieve users in group", e);
                return new List<string>();
            }
        }

        private List<string> GetUsers()
        {
            try
            {
                var connector = GetStardogConnector();
                var query = $"SELECT DISTINCT ?user  {{ ?user a foaf:Person}}";
                var results = (SparqlResultSet)connector.Query(query);
                var users = new List<string>();
                foreach (var result in results)
                {
                    if (result["user"] != null)
                    {
                        var userId = result["user"]?.ToString().Replace($"{_data.BaseObjectUrl}/Users/", "");
                        users.Add(userId);
                    }
                }
                return users.Select(u=>ConvertUserUrlToId(u)).ToList();
            }
            catch (Exception e)
            {
                _logger.Error("Could not retrieve users in group", e);
                return new List<string>();
            }
        }

        public bool GroupExists(string groupName, string userId)
        {
            try
            {
                var connector = GetStardogConnector();
                var query = $"SELECT ?group WHERE {{ ?group a foaf:Group;" +
                                                    $" foaf:name '{groupName}';" +
                                                    $" foaf:maker <{GetUserObjectUrl(_data.BaseObjectUrl, userId)}> }}";
                var result = (SparqlResultSet)connector.Query(query);
                return result.Count > 0;
            }
            catch (Exception e)
            {
                _logger.Error("Could not check if group exists", e);
                return false;
            }
        }

        public bool GroupExists(string groupId)
        {
            try
            {
                var connector = GetStardogConnector();
                var query = $"SELECT ?group WHERE {{?group a foaf:Group; " +
                                                    $"foaf:openid '{groupId}' }}";
                var result = (SparqlResultSet)connector.Query(query);
                return result.Count > 0;
            }
            catch (Exception e)
            {
                _logger.Error("Could not check if group exists", e);
                return false;
            }
        }

        public bool GroupHasClaim(string groupId, string claim)
        {
            try
            {
                var connector = GetStardogConnector();
                var query = $"SELECT ?group WHERE {{?group foaf:topic_interest <{claim}>;" +
                                                        $" foaf:openid '{groupId}'}}";
                var result = (SparqlResultSet)connector.Query(query);
                return result.Count > 0;
            }
            catch (Exception e)
            {
                _logger.Error("Could not check if group has device", e);
                return false;
            }
        }

        public bool RemoveClaimFromGroup(string groupId, string claim)
        {
            try
            {
                var connector = GetStardogConnector();
                var query = $"DELETE DATA {{<{GetGroupObjectUrl(_data.BaseObjectUrl, groupId.ToString())}> foaf:topic_interest <{claim}>}}";
                connector.Update(query);
                return true;
            }
            catch (Exception e)
            {
                _logger.Error("Could not remove device from group", e);
                return false;
            }
        }

        public bool RemoveUserFromGroup(string userId, string groupId)
        {
            try
            {
                var connector = GetStardogConnector();
                var query = $"DELETE DATA {{<{GetUserObjectUrl(_data.BaseObjectUrl, userId.ToString())}> foaf:member <{GetGroupObjectUrl(_data.BaseObjectUrl, groupId.ToString())}>}}";
                connector.Update(query);
                return true;
            }
            catch (Exception e)
            {
                _logger.Error("Could not remove use from group", e);
                return false;
            }
        }

        public bool UserIsInGroup(string groupId, string userId)
        {
            try
            {
                var connector = GetStardogConnector();
                var query = $"SELECT ?user ?group WHERE {{?user foaf:member ?group." +
                                                        $" ?group foaf:openid '{groupId}'." +
                                                        $" ?user foaf:openid '{userId}'}}";
                var result = (SparqlResultSet)connector.Query(query);
                return result.Count > 0;
            }
            catch (Exception e)
            {
                _logger.Error("Could not check if user is in group", e);
                return false;
            }
        }

        private List<Group> AssembleQueryResult(SparqlResultSet resultSet)
        {
            var resultConcepts = new List<Group>();
            foreach (var result in resultSet)
            {

                resultConcepts.Add(AssembleResult(result));
            }
            return resultConcepts;
        }

        private Group AssembleResult(SparqlResult result)
        {
            var ownerId = result["owner"]?.ToString().Replace($"{_data.BaseObjectUrl}/Users/", "");
            var concept = new Group
            {
                GroupId = result["openid"]?.ToString(),
                GroupName = result["name"]?.ToString(),
                OwnerId = ownerId
            };
            return concept;
        }
    }
}
