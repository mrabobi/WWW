using SmartHome.Stardog.Models.Users;
using System;
using System.Collections.Generic;

namespace SmartHome.Stardog.Interfaces
{
    public interface IUserService
    {
        List<UserModel> GetAll();
        bool UserExists(string userId);
        UserModel GetById(string userId);
        UserModel AddUser(UserModel user);
        bool DeleteUser(string userId);
        bool CheckIfUsersAreFriends(string firstUser, string secondUser);
        bool AddFriend(string firstUser, string secondUser);
        bool RemoveFriend(string firstUser, string secondUser);
        public List<UserModel> GetAvailableFriends(string userId);
        List<UserModel> GetFriends(string userId);
    }
}
