using System.ComponentModel.DataAnnotations;
using System.ComponentModel;

namespace IdentityExample.Web.ViewModels
{
    public class PasswordChangeViewModel
    {
        [Required(ErrorMessage = "Eski parola alanı boş geçilemez.")]
        [DisplayName("Eski Parola :")]
        public string OldPassword { get; set; }

        [Required(ErrorMessage = "Yeni parola alanı boş geçilemez.")]
        [DisplayName("Yeni Parola :")]
        [MinLength(6, ErrorMessage = "Şifreniz en az 6 karakter olabilir.")]
        public string NewPassword { get; set; }

        [Required(ErrorMessage = "Yeni parola tekrar alanı boş geçilemez.")]
        [Compare(nameof(NewPassword), ErrorMessage = "Parolalar uyuşmuyor.")]
        [DisplayName("Yeni Parola Tekrar :")]
        public string NewPasswordConfirm { get; set; }
    }
}