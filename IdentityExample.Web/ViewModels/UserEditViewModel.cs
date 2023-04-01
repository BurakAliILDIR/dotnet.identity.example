using System.ComponentModel.DataAnnotations;
using System.ComponentModel;
using IdentityExample.Web.Models;

namespace IdentityExample.Web.ViewModels
{
    public class UserEditViewModel
    {
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


        [DataType(DataType.Date)]
        [DisplayName("Doğum Tarihi :")]
        public DateTime? BirthDate { get; set; }

        [DisplayName("Şehir :")]
        public string? City { get; set; }

        [DataType(DataType.Upload)]
        [DisplayName("Profil Fotoğrafı :")]
        public IFormFile? Picture { get; set; }

        [DisplayName("Cinsiyet :")]
        public Gender? Gender { get; set; }
    }
}