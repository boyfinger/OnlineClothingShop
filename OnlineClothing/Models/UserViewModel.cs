namespace OnlineClothing.Models
{
    public class UserViewModel
    {
        public string UserName { get; set; }
        public string Email { get; set; }
        public string FullName { get; set; }
        public string PhoneNumber { get; set; }
        public string? AvatarUrl { get; set; }
        public int Gender { get; set; }
        public DateOnly DateOfBirth { get; set; }
        public string Address { get; set; }

        public IFormFile? AvatarFile { get; set; }
    }
}
