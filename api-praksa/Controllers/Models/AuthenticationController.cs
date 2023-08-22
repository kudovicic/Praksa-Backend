using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using System.Data;

namespace api_praksa.Controllers.Models
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthenticationController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        public AuthenticationController(IConfiguration configuration)
        {
            _configuration = configuration;
            
        }

        [HttpPost]
        [Route("Registration")]

        public string registration(Register registration)
        {
            var formValidator = new FormValidation();

            // Provjera duljine korisničkog imena
            if (!formValidator.ValidateFirstname(registration.Firstname) ||
                !formValidator.ValidateLastname(registration.Lastname))
            {
                return "The length of the username/lastname exceeds the allowed limit or contains forbidden characters.";
            }

            // Provjera duljine e-mail adrese
            if (!formValidator.ValidateEmail(registration.Email))
            {
                return "The length of the email address exceeds the allowed limit or is not in the correct format.";
            }

            // Provjera duljine lozinke
            if (!formValidator.ValidatePassword(registration.Password))
            {
                return "The length of the password exceeds the allowed limit or is not strong enough.";
            }
            try
            {
                using (SqlConnection con = new SqlConnection(_configuration.GetConnectionString("SqlConnection")))
                {
                    con.Open();
                    using (SqlTransaction transaction = con.BeginTransaction())
                    {

                        string insertUserQuery = "INSERT INTO [User] (Firstname, Lastname, Email, Password, LastLoginTime) VALUES (@Firstname, @Lastname, @Email, @Password, @LastLoginTime); SELECT SCOPE_IDENTITY();";
                        using (SqlCommand cmd = new SqlCommand(insertUserQuery, con, transaction))
                        {
                            cmd.Parameters.AddWithValue("@Firstname", registration.Firstname);
                            cmd.Parameters.AddWithValue("@Lastname", registration.Lastname);
                            cmd.Parameters.AddWithValue("@Email", registration.Email);
                            cmd.Parameters.AddWithValue("@Password", registration.Password);
                            cmd.Parameters.AddWithValue("@LastLoginTime", DateTime.Now);

                            int userId = Convert.ToInt32(cmd.ExecuteScalar());

                            if (userId > 0)
                            {
                                string insertUserRoleQuery = "INSERT INTO User_Role (IdRola, IdUser) VALUES (@IdRola, @IdUser)";
                                using (SqlCommand userRoleCmd = new SqlCommand(insertUserRoleQuery, con, transaction))
                                {
                                    userRoleCmd.Parameters.AddWithValue("@IdRola", 2);
                                    userRoleCmd.Parameters.AddWithValue("@IdUser", userId);

                                    int rowsAffected = userRoleCmd.ExecuteNonQuery();

                                    if (rowsAffected > 0)
                                    {
                                        transaction.Commit();
                                        return "Successful registration!";
                                    }
                                }
                            }
                        }
                    }

                }
                return "Error";
            }catch (Exception ex)
            {
                return "Error in SQL.";
            }

        }



        [HttpPost]
        [Route("Login")]

        public string login(Login login)
        {
            try 
            {
            var formValidator = new FormValidation();
            if (!formValidator.ValidateEmail(login.Email))
            {
                return "The length of the email address exceeds the allowed limit or is not in the correct format.";
            }
            if (!formValidator.ValidatePassword(login.Password))
            {
                return "The length of the password exceeds the allowed limit or is not strong enough.";
            }
            SqlConnection con = new SqlConnection(_configuration.GetConnectionString("SqlConnection").ToString());
            SqlDataAdapter da = new SqlDataAdapter("SELECT * FROM [User] WHERE Email = '" + login.Email + "' AND Password = '" + login.Password + "'  ", con);
            DataTable dt = new DataTable();
            da.Fill(dt);
            if(dt.Rows.Count > 0)
            {
                int userId = Convert.ToInt32(dt.Rows[0]["Id"]); 
                string token = Guid.NewGuid().ToString(); 
                TimeSpan timeout = TimeSpan.FromHours(8); 
                DateTime LoginTime = DateTime.Now; 
                DateTime Timeout = LoginTime.Add(timeout);


                using (SqlConnection conn = new SqlConnection(_configuration.GetConnectionString("SqlConnection").ToString()))
                {
                    conn.Open();
                    var updateCmd = new SqlCommand("UPDATE [User] SET lastLoginTime = @lastLoginTime WHERE Id = @userId", conn);
                    updateCmd.Parameters.AddWithValue("@lastLoginTime", LoginTime);
                    updateCmd.Parameters.AddWithValue("@userId", userId);
                    updateCmd.ExecuteNonQuery();

                    var deleteCmd = new SqlCommand("DELETE FROM [Session] WHERE idUser = @userId", conn);
                    deleteCmd.Parameters.AddWithValue("@userId", userId);
                    deleteCmd.ExecuteNonQuery();

                    var insertCmd = new SqlCommand("INSERT INTO [Session] (idUser, sessionLoginTime, sessionTimeout, tokenGuid) VALUES (@idUser, @sessionLoginTime, @sessionTimeout, @tokenGuid)", conn);
                    insertCmd.Parameters.AddWithValue("@idUser", userId);
                    insertCmd.Parameters.AddWithValue("@sessionLoginTime", LoginTime);
                    insertCmd.Parameters.AddWithValue("@sessionTimeout", Timeout);
                    insertCmd.Parameters.AddWithValue("@TokenGuid", token);
                    insertCmd.ExecuteNonQuery();
                    conn.Close();
                }

                return "Valid user.";

            }
            else
            {
                return "Invalid user.";
            }
            
            }catch (Exception ex)
            {
                return "Error in SQL";
            }
        }
  
    }
}
