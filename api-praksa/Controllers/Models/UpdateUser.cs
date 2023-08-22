namespace api_praksa.Controllers.Models
{
    public class UpdateUser
    {
        public string Firstname { get; set; }
        public string Lastname { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        
        public int RoleId { get; set; }
    }
}
