using api_praksa.Services.UserService;
using Microsoft.AspNetCore.Mvc;
using api_praksa.Services;
using api_praksa.Controllers;
using Microsoft.AspNetCore.Authorization;
using api_praksa.Controllers.Models;

namespace api_praksa.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
  
    public class UsersController : ControllerBase
    {

        private readonly IUserService _userService;
        
        //konstruktor
        public UsersController(IUserService userService)
        {
            _userService = userService;
        }
       
        
        [HttpGet("Get")]
        [AdminOnly]
        public async Task<IActionResult> GetUsers()
        {
            try
            {
                var users = await _userService.GetUsers();
                return Ok(users);
            }
            catch (Exception ex)
            {
                //log error
                return BadRequest("Error in SQL");
            }
        }


        [HttpGet("Get/{id}", Name = "UserById")]
        [AdminOnly]

        public async Task<IActionResult> GetUser(int id)
        {
            try
            {
                var user = await _userService.GetUser(id);
                if (user == null)
                    return NotFound();

                return Ok(user);
            }
            catch (Exception ex)
            {
                //log error
                return BadRequest("Error in SQL");
            }
        }


        [HttpPost("Create")]
        [AdminOnly]
        public async Task<IActionResult> CreateUser([FromBody]CreateUser user)
        {
            try
            {
                var formValidator = new FormValidation();

                // Provjera duljine korisničkog imena
                if (!formValidator.ValidateFirstname(user.Firstname) ||
                    !formValidator.ValidateLastname(user.Lastname))
                {
                    return BadRequest($"The length of the username/lastname exceeds the allowed limit or contains forbidden characters.");
                }

                // Provjera duljine e-mail adrese
                if (!formValidator.ValidateEmail(user.Email))
                {
                    return BadRequest($"The length of the email address exceeds the allowed limit or is not in the correct format.");
                }

                // Provjera duljine lozinke
                if (!formValidator.ValidatePassword(user.Password))
                {
                    return BadRequest($"The length of the password exceeds the allowed limit or is not strong enough.");
                }

                var createdUser = await _userService.CreateUser(user);
                return CreatedAtRoute("UserById", new { id = createdUser.Id }, createdUser);
            }

            catch (Exception ex)
            {
                //log error
                return BadRequest("Error in SQL");
            }
        }

       
        [HttpPut("Update/{id}")]
        [AdminOnly]
        public async Task<IActionResult> UpdateUser(int id, UpdateUser user)
        {
            try
            {
                var dbUser = await _userService.GetUser(id);
                if (dbUser == null)
                    return NotFound();
                var formValidator = new FormValidation();

                // Validacija imena
                if (!formValidator.ValidateFirstname(user.Firstname))
                    return BadRequest("Invalid first name.");

                // Validacija prezimena
                if (!formValidator.ValidateLastname(user.Lastname))
                    return BadRequest("Invalid last name.");

                // Validacija emaila
                if (!formValidator.ValidateEmail(user.Email))
                    return BadRequest("Invalid email.");

                // Validacija lozinke
                if (!formValidator.ValidatePassword(user.Password))
                    return BadRequest("Invalid password.");
               
                //Validacija id-a
                if (!formValidator.ValidateRoleId(user.RoleId))
                    return BadRequest("Invalid ID.");
                
                await _userService.UpdateUser(id, user);
                return Ok("The user data has been successfully updated.");
            }
            catch (Exception ex)
            {
                //log error
                return BadRequest("Error in SQL");
            }
        }


        [HttpDelete("Delete/{id}")]
        [AdminOnly]
        public async Task<IActionResult> DeleteUser(int id)
        {
            try
            {
                var dbUser = await _userService.GetUser(id);
                if (dbUser== null)
                    return NotFound();
                await _userService.DeleteUser(id);
                return Ok("The user has been successfully deleted.");
            }
            catch (Exception ex)
            {
                //log error
                return BadRequest("Error in SQL");
            }
        }


      
    }
}

