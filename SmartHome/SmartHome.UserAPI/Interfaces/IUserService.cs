using SmartHome.UserAPI.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SmartHome.UserAPI.Interfaces
{
    public interface IUserService
    {
        List<User> GetAll();
        bool UserExists(Guid userId);
        User GetById(Guid userId);
        User AddUser(User user);
        bool DeleteUser(Guid userId);
        User UpdateUser(User user);
    }
}
