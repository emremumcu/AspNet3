using System.ComponentModel.DataAnnotations;

namespace AspNet3.ViewModels
{
    public class RegisterViewModel : LoginViewModel
    {
        [DataType(DataType.Password), Compare(nameof(Password))]
        [Display(Name = "Şifre Tekrar")]
        [Required(ErrorMessage = "Şifrenizi tekrar giriniz")]
        public string ConfirmPassword { get; set; }
    }
}
