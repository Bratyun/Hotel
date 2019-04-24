using Contract.Consts;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Security.Claims;
using System.Text;

namespace Contract.Models
{
    public class User
    {
        [Required]
        public int Id { get; set; }
        [Required]
        public string FirstName { get; set; }
        [Required]
        public string Login { get; set; }
        [Required]
        [DataType(DataType.Password)]
        [Range(6, 21, ErrorMessage = "Invalid password. Needs value between 6 and 21")]
        public string Password { get; set; }
        [DataType(DataType.PhoneNumber, ErrorMessage = "Invelid phone number")]
        public string Phone { get; set; }
        [DataType(DataType.EmailAddress, ErrorMessage = "Invalid email")]
        public string Email { get; set; }
        [Required]
        public string Role { get; set; }

        public User()
        {

        }

        public User(int id, string firstName, string phone, string email, string role, string login, string password)
        {
            Id = id;
            FirstName = firstName;
            Login = login;
            Password = password;
            Email = email;
            Role = role;
            Phone = phone;
        }

        public static User GetUser(ClaimsPrincipal claims)
        {
            User user = new User
            {
                Id = int.Parse(claims.FindFirst(TokenClaims.ID)?.Value),
                FirstName = claims.FindFirst(TokenClaims.Name)?.Value,
                Role = claims.FindFirst(ClaimTypes.Role)?.Value,
            };
            return user;
        }
    }
}
