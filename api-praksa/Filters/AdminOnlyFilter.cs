using api_praksa.Services.UserService;
using Dapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Data.SqlClient;

namespace api_praksa.Filters
{
    public class AdminOnlyFilter: IAsyncAuthorizationFilter
    {
        
        private readonly IConfiguration _configuration;
        private readonly string _connectionString;

        public AdminOnlyFilter(IConfiguration configuration)
        {
            _configuration = configuration;
            _connectionString = _configuration.GetConnectionString("SqlConnection");
            
        }

        public async Task OnAuthorizationAsync(AuthorizationFilterContext context)
        {
            bool isRequestAuthorized = await IsRequestAuthorizedAsync(context.HttpContext.Request.Headers["X-Token"]);
            if (!isRequestAuthorized)
            {
                context.Result = new UnauthorizedResult();
            }
        }

        private async Task<bool> IsRequestAuthorizedAsync(string sessionToken)
        {
            int status = 100;
          
            bool isAdmin = false;
            bool tokenExists = false;
            if (!string.IsNullOrEmpty(sessionToken))

            {
                using (SqlConnection conn = new SqlConnection(_connectionString))
                {
                    try
                    {
                        await conn.OpenAsync();
                        var tokenQuery = "SELECT COUNT(*) FROM [Session] WHERE [tokenGuid] = @tokenGuid";
                        int tokenCount = await conn.ExecuteScalarAsync<int>(tokenQuery, new { tokenGuid = sessionToken });
                        tokenExists = (tokenCount > 0);
                        if (tokenExists)
                        {
                            var roleQuery = @"
                                   SELECT [Role].[Name] 
                                   FROM [Session]
                                   INNER JOIN [User] ON [Session].[idUser] = [User].[Id]
                                   INNER JOIN User_Role ON [User].Id = User_Role.idUser
                                   INNER JOIN Role ON User_Role.idRola = Role.Id
                                   WHERE [Session].[TokenGuid] = @tokenGuid";

                            var result = await conn.QuerySingleOrDefaultAsync<string>(roleQuery, new { tokenGuid = sessionToken });
                            isAdmin = "Administrator" == result;
                        }
                    }
                    catch (Exception ex)
                    {
                        return false;
                    }
                }
            }
            if (isAdmin)
            {
                return true;
            }

            return false;
        }
    }
}
