using System.ComponentModel.DataAnnotations;

namespace Trackly.Models
{
    public class RegistrationModel
    {
        [Required(ErrorMessage = "Korisničko ime mora biti upisano.")]
        public string? UserName { get; set; }

        [Required(ErrorMessage = "E-mail mora biti upisan.")]
        [EmailAddress(ErrorMessage ="E-mail nije dobar.")]
        public string? Email { get; set; }

        [Required(ErrorMessage = "Lozinka mora biti upisana.")]
        [MinLength(8, ErrorMessage = "Lozinka mora sadržavati minimalno 8 znakova.")]
        public string? Password { get; set; }

        [Compare("Password", ErrorMessage = "Lozinka i potvrda lozinke nisu iste.")]
        public string? ConfirmPassword { get; set; }
    }
}
