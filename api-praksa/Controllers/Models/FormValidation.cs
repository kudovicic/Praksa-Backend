using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using PraksaProjekt.Context;
using System.Text.RegularExpressions;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;

namespace api_praksa.Controllers.Models
{
    public class FormValidation
    {
        public bool ValidateFirstname(string firstname)
        {
            // Provjera duljine korisničkog imena
            if (string.IsNullOrEmpty(firstname) || firstname.Length < 1 || firstname.Length > 50)
                return false;

            // Provjera dozvoljenih znakova za korisničko ime 
            if (!Regex.IsMatch(firstname, "^[a-zA-ZčćžČĆŽ]+$"))
                return false;
            return true;
        }

        public bool ValidateLastname(string lastname)
        {
            // Provjera duljine prezimena
            if (string.IsNullOrEmpty(lastname) || lastname.Length < 1 || lastname.Length > 50)
                return false;

            // Provjera dozvoljenih znakova za prezime
            if (!Regex.IsMatch(lastname, "^[a-zA-ZčćžČĆŽ]+$"))
                return false;
            return true;
        }

        public bool ValidatePassword(string password)
        {
            // Provjera duljine lozinke
            if (string.IsNullOrEmpty(password) || password.Length < 1 || password.Length > 50)
                return false;

            // Provjera jačine lozinke (barem jedno veliko slovo, jedno malo slovo i jedna brojka)
            if (!Regex.IsMatch(password, @"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d).+$"))
                return false;

            return true;
        }

        public bool ValidateEmail(string email)
        {
            // Provjera ispravnosti e-mail adrese
            if (string.IsNullOrEmpty(email) || email.Length < 1 ||  email.Length > 50 || !Regex.IsMatch(email, @"^[^@\s]+@[^@\s]+\.[^@\s]+$"))
                return false;

            return true;
        }

        public bool ValidateAuthorId(int id)
        {
            if (id <= 0)
                return false;
            return true;

        }

        public bool ValidatePostTitle(string title)
        {
            
            if (string.IsNullOrEmpty(title) || title.Length < 1 || title.Length > 255)
                return false;
            return true;
        }

        public bool ValidateRoleId(int id)
        {
            if (id == 1 || id == 2)
                return true;

            return false;
        }


    }
}
