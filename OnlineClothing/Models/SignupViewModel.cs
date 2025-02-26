using System.ComponentModel.DataAnnotations;

namespace OnlineClothing.Models
{
    public class SignUpViewModel
    {
        public string FullName { get; set; }
        public string Email { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        public string ConfirmPassword { get; set; }
        public string PhoneNumber { get; set; }
        public string Gender { get; set; }
        public DateOnly DateOfBirth { get; set; }
        public string Address { get; set; }
        public string UserType { get; set; }
    }

}
