using System.ComponentModel.DataAnnotations;

namespace OnlineClothing.Models
{
    public class ForgotModelView
    {
        [Required, EmailAddress]
        public string Email { get; set; }
    }
}
