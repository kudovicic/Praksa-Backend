using Microsoft.AspNetCore.Identity;
using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace api_praksa.Controllers.Models
{
	public class User : IdentityUser<int>
	{
        
        public int Id { get; set; }
		public string Firstname { get; set; }
        public string Lastname { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
		public DateTime lastLoginTime { get; set; }

		public User()
		{
		}
	}
}

