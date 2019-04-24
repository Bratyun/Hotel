using System.ComponentModel.DataAnnotations;

namespace Web.Models
{
    public class UserWithRoleViewModel
    {
        [Required]
        public int Id { get; set; }
        [Required]
        public string Role { get; set; }
        [Required]
        public string FirstName { get; set; }
        [Required]
        public string Login { get; set; }
        [DataType(DataType.PhoneNumber, ErrorMessage = "Invalid phone number")]
        public string PhoneNumber { get; set; }
        [DataType(DataType.EmailAddress, ErrorMessage = "Invalid email")]
        public string Email { get; set; }
        public string Phone { get; set; }
        [Required]
        [Range(6, 21, ErrorMessage = "Invalid password. Needs value between 6 and 21")]
        public string Password { get; set; }
        
        

    }
}