using System.ComponentModel.DataAnnotations;

namespace Trackly.Models
{
    public class RegistrationModel
    {
        [Required(ErrorMessage = "Username is required.")]
        public string? Username { get; set; }

        [Required(ErrorMessage = "E-mail is required.")]
        [EmailAddress(ErrorMessage ="E-mail is not valid.")]
        public string? Email { get; set; }

        [Required(ErrorMessage = "Password is required.")]
        [MinLength(8, ErrorMessage = "Password needs to contain at least 8 characters.")]
        public string? Password { get; set; }

        [Compare("Password", ErrorMessage = "Passwords do not match.")]
        public string? ConfirmPassword { get; set; }
    }
}
