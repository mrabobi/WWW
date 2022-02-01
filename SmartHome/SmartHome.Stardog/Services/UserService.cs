using Serilog;
using SmartHome.Stardog.Interfaces;
using SmartHome.Stardog.Models.Users;
using System;
using System.Collections.Generic;
using System.Linq;
using VDS.RDF.Query;

namespace SmartHome.Stardog.Services
{
    public class UserService : StardogService, IUserService
    {
        public UserService(StardogData data, ILogger logger) : base(data, logger)
        {

        }

        public bool AddFriend(string firstUser, string secondUser)
        {
            try
            {
                var connector = GetStardogConnector();
                var query = $"INSERT DATA {{<{GetUserObjectUrl(_data.BaseObjectUrl, firstUser)}> foaf:knows <{GetUserObjectUrl(_data.BaseObjectUrl,secondUser)}>}}";
                connector.Update(query);
                return true;
            }
            catch (Exception e)
            {
                _logger.Error("Could not add friend", e);
                return false;
            }
        }

        public UserModel AddUser(UserModel user)
        {
            try
            {
                var connector = GetStardogConnector();
                var query = $"INSERT DATA {{<{GetUserObjectUrl(_data.BaseObjectUrl, user.UserId)}> a foaf:Person;" +
                    $"foaf:familyName '{user.LastName}';" +
                    $"foaf:givenName '{user.FirstName}';" +
                    $"foaf:openid '{user.UserId}';" +
                    $"foaf:mbox '{user.Email}';" +
                    $"foaf:accountName '{user.DisplayName}'}}";
                connector.Update(query);
                return user;
            }
            catch (Exception e)
            {
                _logger.Error("Could not add User", e);
                return null;
            }
        }

        public bool CheckIfUsersAreFriends(string firstUser, string secondUser)
        {
            try
            {
                var connector = GetStardogConnector();
                var query = $"SELECT DISTINCT *  {{?user1 foaf:knows ?user2. FILTER(STR(?user1) = '{GetUserObjectUrl(_data.BaseObjectUrl,firstUser)}' && STR(?user2) = '{GetUserObjectUrl(_data.BaseObjectUrl,secondUser)}')}}";
                var result = (SparqlResultSet)connector.Query(query);
                return result.Count>0;
            }
            catch (Exception e)
            {
                _logger.Error("Could not add friend", e);
                return false;
            }
        }

        public bool DeleteUser(string userId)
        {
            try
            {
                var user = GetById(userId);
                var connector = GetStardogConnector();
                var query = $"DELETE DATA {{<{GetUserObjectUrl(_data.BaseObjectUrl, user.UserId)}> a foaf:Person;" +
                    $"foaf:familyName '{user.LastName}';" +
                    $"foaf:givenName '{user.FirstName}';" +
                    $"foaf:openid '{user.UserId}';" +
                    $"foaf:mbox '{user.Email}';" +
                    $"foaf:accountName '{user.DisplayName}'}}";
                connector.Update(query);
                return true;
            }
            catch (Exception e)
            {
                _logger.Error("Could not add User", e);
                return false;
            }
        }

        public List<UserModel> GetAll()
        {
            try
            {
                var connector = GetStardogConnector();
                var query = $"SELECT DISTINCT *  {{?user a foaf:Person; " +
                    $"foaf:familyName ?familyName; " +
                    $"foaf:givenName?givenName; " +
                    $"foaf:openid ?openid; " +
                    $"foaf:mbox ?mbox; " +
                    $"foaf:accountName ?accountName}}";
                var result = (SparqlResultSet)connector.Query(query);
                return AssembleQueryResult(result);
            }
            catch (Exception e)
            {
                _logger.Error($"Could not retrieve users", e);
                return null;
            }
        }

        public List<UserModel> GetAvailableFriends(string userId)
        {
            try
            {
                var connector = GetStardogConnector();
                var query = $"SELECT DISTINCT ?userId WHERE {{?user a foaf:Person; foaf:openid ?userId " +
                    $"FILTER(NOT EXISTS {{<{GetUserObjectUrl(_data.BaseObjectUrl, userId)}> foaf:knows ?user}} && STR(?userId)!='{userId}')}}";
                var results = (SparqlResultSet)connector.Query(query);
                var users = new List<UserModel>();
                foreach (var result in results)
                {
                    if (result["userId"] != null)
                    {
                        var user = GetById(result["userId"].ToString());
                        if (user != null)
                        {
                            users.Add(user);
                        }
                    }
                }
                return users;
            }
            catch (Exception e)
            {
                _logger.Error("Could not retrieve available friends", e);
                return new List<UserModel>();
            }
        }

        //public List<UserModel> GetAvailableFriends(string userId)
        //{
        //    try
        //    {
        //        var friends = GetFriends(userId);
        //        var all = GetAll();
        //        var available = all.Where(u => friends.All(u2 => u2.UserId != u.UserId) && u.UserId != userId).ToList();
        //        return available;
        //    }
        //    catch (Exception e)
        //    {
        //        _logger.Error("Could not retrieve available friends", e);
        //        return new List<UserModel>();
        //    }
        //}

        public List<UserModel> GetFriends(string userId)
        {
            try
            {
                var connector = GetStardogConnector();
                var query = $"SELECT DISTINCT ?user {{?user1 foaf:knows ?user. FILTER(STR(?user1) = '{GetUserObjectUrl(_data.BaseObjectUrl, userId)}')}}";
                var results = (SparqlResultSet)connector.Query(query);
                var users = new List<UserModel>();
                foreach (var result in results)
                {
                    if (result["user"] != null)
                    {
                        var friendId = ConvertUserUrlToId(result["user"].ToString());
                        var user = GetById(friendId);
                        if (user != null)
                        {
                            users.Add(user);
                        }
                    }
                }
                return users;
            }
            catch (Exception e)
            {
                _logger.Error("Could not retrieve available friends", e);
                return new List<UserModel>();
            }
        }

        public UserModel GetById(string userId)
        {
            try
            {
                var connector = GetStardogConnector();
                var query = $"SELECT DISTINCT *  {{?user a foaf:Person; " +
                    $"foaf:familyName ?familyName; " +
                    $"foaf:givenName?givenName; " +
                    $"foaf:openid ?openid; " +
                    $"foaf:mbox ?mbox; " +
                    $"foaf:accountName ?accountName " +
                    $"FILTER(STR(?openid) = '{userId}')}}";
                var result = (SparqlResultSet)connector.Query(query);
                return AssembleQueryResult(result).FirstOrDefault();
            }
            catch (Exception e)
            {
                _logger.Error($"Could not retrieve user with id {userId}", e);
                return null;
            }
        }

        public bool RemoveFriend(string firstUser, string secondUser)
        {
            try
            {
                var connector = GetStardogConnector();
                var query = $"DELETE DATA {{<{GetUserObjectUrl(_data.BaseObjectUrl, firstUser)}> foaf:knows <{GetUserObjectUrl(_data.BaseObjectUrl, secondUser)}>}}";
                connector.Update(query);
                return true;
            }
            catch (Exception e)
            {
                _logger.Error("Could not add friend", e);
                return false;
            }
        }

        public bool UserExists(string userId)
        {
            try
            {
                var connector = GetStardogConnector();
                var query = $"SELECT ?user WHERE {{?user foaf:openid '{userId}' }}";
                var result = (SparqlResultSet)connector.Query(query);
                return result.Count > 0;
            }
            catch (Exception e)
            {
                _logger.Error("Could not check if user exists", e);
                return false;
            }
        }

        private List<UserModel> AssembleQueryResult(SparqlResultSet resultSet)
        {
            var resultConcepts = new List<UserModel>();
            foreach (var result in resultSet)
            {

                resultConcepts.Add(AssembleResult(result));
            }
            return resultConcepts;
        }

        private UserModel AssembleResult(SparqlResult result)
        {
            var concept = new UserModel
            {
                UserId = result["openid"]?.ToString(),
                LastName = result["familyName"]?.ToString(),
                FirstName = result["givenName"]?.ToString(),
                DisplayName = result["accountName"]?.ToString(),
                Email = result["mbox"]?.ToString()
            };
            return concept;
        }
    }
}
