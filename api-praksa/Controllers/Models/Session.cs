namespace api_praksa.Controllers.Models
{
    public class Session
    {
        public int id { get; set; }
        public int idUser { get; set; }
        public DateTime sessionLoginTime { get; set; }
        public DateTime sessionTimeout { get; set; }
        public string tokenGuid { get; set; }


    }
}
