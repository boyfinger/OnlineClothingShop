using System.ComponentModel.DataAnnotations;

namespace OnlineClothing.Models
{
    public class LoginViewModel
    {
        [Required]
        [EmailAddress]
        public string LoginEmail { get; set; }

        [Required]
        [DataType(DataType.Password)]
        public string LoginPassword { get; set; }

        public string UserType { get; set; }  // "customer" or "seller"
    }

}
