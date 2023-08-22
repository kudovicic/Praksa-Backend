using System;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;


namespace api_praksa.Services.UserService
{
    public interface IUserService
    {
        Task<IEnumerable<User>> GetUsers();
        Task<User> GetUser(int id);
        public Task<User> CreateUser(CreateUser user);
        public Task UpdateUser(int id, UpdateUser user);
        public Task DeleteUser(int id);
        
       
    }
}
