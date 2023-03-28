using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace IdentityExample.Web.ViewModels
{
    public class SignUpViewModel
    {
        public SignUpViewModel()
        {
        }

        public SignUpViewModel(string userName, string email, string phoneNumber, string password)
        {
            UserName = userName;
            Email = email;
            PhoneNumber = phoneNumber;
            Password = password;
        }

        [Required(ErrorMessage = "Kullanıcı adı alanı boş geçilemez.")]
        [DisplayName("Kullanıcı Adı :")]
        public string UserName { get; set; }


        [Required(ErrorMessage = "Email alanı boş geçilemez.")]
        [EmailAddress(ErrorMessage = "Email formatı doğru değil.")]
        [DisplayName("Email :")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Telefon numarası alanı boş geçilemez.")]
        [DisplayName("Telefon :")]
        public string PhoneNumber { get; set; }

        [Required(ErrorMessage = "Parola alanı boş geçilemez.")]
        [DisplayName("Parola :")]
        public string Password { get; set; }

        [Required(ErrorMessage = "Parola tekrar alanı boş geçilemez.")]
        [Compare(nameof(Password),ErrorMessage = "Parolalar uyuşmuyor.")]
        [DisplayName("Parola Tekrar :")]
        public string PasswordConfirm { get; set; }
    }
}