using System.ComponentModel.DataAnnotations;
using System.ComponentModel;

namespace IdentityExample.Web.ViewModels
{
    public class ResetPasswordViewModel
    {

        [Required(ErrorMessage = "Email alanı boş geçilemez.")]
        [EmailAddress(ErrorMessage = "Email formatı doğru değil.")]
        [DisplayName("Email :")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Yeni parola alanı boş geçilemez.")]
        [DisplayName("Yeni Parola :")]
        public string Password { get; set; }

        [Required(ErrorMessage = "Yeni parola tekrar alanı boş geçilemez.")]
        [Compare(nameof(Password), ErrorMessage = "Parolalar uyuşmuyor.")]
        [DisplayName("Yeni Parola Tekrar :")]
        public string PasswordConfirm { get; set; }
    }
}
