using AspNet3.Models;
using System;
using System.ComponentModel.DataAnnotations;

namespace AspNet3.ViewModels
{
    public class LoginViewModel
    {
        [Display(Name = "Kullanıcı Adı")]
        [Required(ErrorMessage = "Kullanıcı Adınızı giriniz"), DataType(DataType.Text)]
        [StringLength(30, ErrorMessage = "{0} en az {2} en fazla {1} karakter olmalıdır.", MinimumLength = 3)]
        public string Username { get; set; }

        [Display(Name = "Şifre")]
        [Required(ErrorMessage = "Şifrenizi giriniz"), DataType(DataType.Password)]
        public string Password { get; set; }

        [Display(Name = "Güvenlik Kodu")]
        public CaptchaResult Captcha { get; set; }

        [Display(Name = "Beni Hatırla")]
        public Boolean RememberMe { get; set; }
    }
}
