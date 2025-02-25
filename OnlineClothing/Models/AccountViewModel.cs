using System.ComponentModel.DataAnnotations;

namespace OnlineClothing.Models
{
    public class AccountViewModel
    {
        [Required]
        [EmailAddress]
        public string LoginEmail { get; set; }

        [Required]
        [DataType(DataType.Password)]
        public string LoginPassword { get; set; }

        // This can be used for regular user login forms to distinguish between "customer" and "seller"
        public string? UserType { get; set; }  // "customer" or "seller" - not needed for admin login, but useful in other cases
    }
}
