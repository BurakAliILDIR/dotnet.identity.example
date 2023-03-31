using System.ComponentModel.DataAnnotations;
using System.ComponentModel;

namespace IdentityExample.Web.ViewModels
{
    public class ForgetPasswordViewModel
    {
        [Required(ErrorMessage = "Email alanı boş geçilemez.")]
        [EmailAddress(ErrorMessage = "Email formatı doğru değil.")]
        [DisplayName("Email :")]
        public string Email { get; set; }
    }
}
