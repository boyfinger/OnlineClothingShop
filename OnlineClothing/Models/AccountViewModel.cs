using System.ComponentModel.DataAnnotations;

namespace OnlineClothing.Models
{
    public class AccountViewModel
    {
        // This property holds the LoginForm data
        public LoginViewModel LoginModel { get; set; }

        // This property holds the SignUpForm data
        public SignUpViewModel SignupModel { get; set; }
    }
}
