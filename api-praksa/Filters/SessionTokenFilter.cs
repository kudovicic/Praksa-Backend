using Dapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Data.SqlClient;

namespace api_praksa.Filters
{
    public class SessionTokenFilter : IAsyncAuthorizationFilter
    {
        private readonly IConfiguration _configuration;
        private readonly string _connectionString;

        public SessionTokenFilter(IConfiguration configuration)
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
            bool tokenExists = false;
            if (!string.IsNullOrEmpty(sessionToken))
            {
                using (SqlConnection conn = new SqlConnection(_connectionString))
                {
                    try
                    {
                        await conn.OpenAsync();
                        var query = "SELECT COUNT(*) FROM [Session] WHERE [tokenGuid] = @tokenGuid";
                        int count = await conn.ExecuteScalarAsync<int>(query, new { tokenGuid = sessionToken });
                        tokenExists = (count > 0);
                    }
                    catch (Exception ex)
                    {
                        return false;
                    }
                }
            }

            return tokenExists;
        }
    }
}
