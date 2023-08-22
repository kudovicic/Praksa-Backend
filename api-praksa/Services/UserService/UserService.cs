using Azure.Messaging;
using Dapper;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using PraksaProjekt.Context;
using System.Data;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;


namespace api_praksa.Services.UserService
{
    public class UserService : IUserService
    {
        private readonly DapperContext _context;
        public UserService(DapperContext context)
        {
            _context = context;
        }

        public async Task<User> CreateUser(CreateUser user)
        {
            var query = "INSERT INTO [User] (Firstname,Lastname,Email,Password) VALUES (@Firstname, @Lastname, @Email,@Password)" +
                "SELECT CAST(SCOPE_IDENTITY() AS int)";

            var parameters = new DynamicParameters();
            parameters.Add("firstname", user.Firstname, DbType.String);
            parameters.Add("lastname", user.Lastname, DbType.String);
            parameters.Add("email", user.Email, DbType.String);
            parameters.Add("password", user.Password, DbType.String);
            

            using (var connection = _context.CreateConnection())
            {
                var id = await connection.QuerySingleAsync<int>(query, parameters);
                var roleId = 2;

                var userRoleQuery = "INSERT INTO User_Role (idUser, idRola) VALUES (@idUser, @idRola)";
                await connection.ExecuteAsync(userRoleQuery, new { idUser = id, idRola = roleId });

                var createdUser = new User
                {
                    Id = id,
                    Firstname = user.Firstname,
                    Lastname = user.Lastname,
                    Email = user.Email,
                    Password = user.Password,
                    
                };
                return createdUser;
            }
        }


        public async Task<User> GetUser(int id)
        {
            var query = "SELECT * FROM dbo.[User] WHERE Id = @Id";

            using (var connection = _context.CreateConnection())
            {
                var user = await connection.QuerySingleOrDefaultAsync<User>(query, new { id });

                return user;
            }
        }

        public async Task<IEnumerable<User>> GetUsers()
        {
            var query = "select * from dbo.[User]";
            using (var connection = _context.CreateConnection())
            {
                var users = await connection.QueryAsync<User>(query);
                return users.ToList();
            }

        }

        public async Task UpdateUser(int id, UpdateUser user)
        {
            var query = "UPDATE [User] SET  FirstName=@FirstName, Lastname=@Lastname, Email=@Email, Password=@Password WHERE Id=@Id;" +
                        "UPDATE User_Role  SET idRola = @RoleId  WHERE idUser = @Id;";

            var parameters = new DynamicParameters();
            parameters.Add("id", id, DbType.Int32);
            parameters.Add("firstname", user.Firstname, DbType.String);
            parameters.Add("lastname", user.Lastname, DbType.String);
            parameters.Add("email", user.Email, DbType.String);
            parameters.Add("password", user.Password, DbType.String);
            
            parameters.Add("RoleId", user.RoleId, DbType.Int32);

            using (var connection = _context.CreateConnection())
            {
                await connection.ExecuteAsync(query, parameters);
            }
        }

        public async Task DeleteUser(int id)
        {
            using (var connection = _context.CreateConnection())
            {
                connection.Open();
                using (var transaction = connection.BeginTransaction())
                {
                    try
                    {
                        var deleteQuery = "DELETE FROM Session WHERE idUser = @id;" +
                            "DELETE FROM Post WHERE AuthorId = @id;" +
                            "DELETE FROM User_Role WHERE idUser = @id;" +
                            "DELETE FROM [User] WHERE id = @id;";

                        await connection.ExecuteAsync(deleteQuery, new { id }, transaction);
                        
                        transaction.Commit();
                    }
                    catch (Exception ex)
                    {
                        throw;
                    }
                }
            }
        }





    }
}
