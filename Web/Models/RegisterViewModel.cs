using Contract.Models;
using System.ComponentModel.DataAnnotations;

namespace Web.Models
{
    public class RegisterViewModel : User
    {
        [Required]
        [DataType(DataType.Password)]
        [Compare("Password", ErrorMessage = "Password don't fit")]
        public string ConfirmPassword { get; set; }
        
    }
}