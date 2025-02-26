using System.ComponentModel.DataAnnotations;

namespace OnlineClothing.Models
{
    public class LoginViewModel
    {
        public string UserType { get; set; }
        public string LoginEmail { get; set; }
        public string LoginPassword { get; set; }
    }

}
