using System.ComponentModel.DataAnnotations;
using System.ComponentModel;

namespace IdentityExample.Web.ViewModels
{
    public class SignInViewModel
    {
        [Required(ErrorMessage = "Email alanı boş geçilemez.")]
        [EmailAddress(ErrorMessage = "Email formatı doğru değil.")]
        [DisplayName("Email :")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Parola alanı boş geçilemez.")]
        [DisplayName("Parola :")]
        public string Password { get; set; }

        public bool RememberMe { get; set; }
    }
}