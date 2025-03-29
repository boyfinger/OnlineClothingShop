using System.Text.RegularExpressions;

namespace OnlineClothing.Utils
{
    public class ValidationUtils
    {
        public static bool IsValidEmail(string email)
        {
            string emailRegex = @"^[a-zA-Z0-9_+&*-]+(?:\.[a-zA-Z0-9_+&*-]+)*@(?:[a-zA-Z0-9-]+\.)+[a-zA-Z]{2,7}$";
            Regex regex = new Regex(emailRegex);
            return regex.IsMatch(email);
        }

        public static bool IsValidFullName(string fullName)
        {
            if (fullName.Length > 100) return false;
            string fullNameRegex = @"^[a-zA-Z]+(\s[a-zA-Z]+)*$";
            Regex regex = new Regex(fullNameRegex);
            return regex.IsMatch(fullName);
        }

        public static bool IsValidGender(string gender)
        {
            return gender == "male" || gender == "female" || gender == "other";
        }

        public static bool IsValidGender(int? gender)
        {
            return gender == 1 || gender == 2 || gender == 3;
        }

        public static bool IsValidMobile(string mobile)
        {
            string mobileRegex = @"^(09|03|05|07|08)\d{8}$";
            Regex regex = new Regex(mobileRegex);
            return regex.IsMatch(mobile);
        }

        //at least one uppercase, one lowercase, one digit
        public static bool IsValidPassword(string password)
        {
            string passwordRegex = @"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)[a-zA-Z\d\W]{8,}$";
            Regex regex = new Regex(passwordRegex);
            return regex.IsMatch(password);
        }

        public static bool IsValidDateOfBirth(DateOnly dateOfBirth)
        {
            DateOnly today = DateOnly.FromDateTime(DateTime.Today);
            int age = today.Year - dateOfBirth.Year;
            if (dateOfBirth > today.AddYears(-age))
            {
                age--;
            }
            return age >= 18;
        }

        public static bool IsValidUserName(string userName)
        {
            if (string.IsNullOrEmpty(userName) || userName.Length < 5 || userName.Length > 20) return false;

            string pattern = @"^[a-zA-Z][a-zA-Z0-9._-]{5,19}$";
            Regex regex = new Regex(pattern);
            if (!regex.IsMatch(userName)) return false;
            return true;
        }

        public static bool IsValidAvatar(IFormFile avatarFile)
        {
            const long maxFileSize = 5 * 1024 * 1024; // 5 MB in bytes
            if (avatarFile.Length > maxFileSize)
                return false;

            string[] validExtensions = { ".jpg", ".jpeg", ".png", ".gif" };
            string fileExtension = Path.GetExtension(avatarFile.FileName).ToLower();
            if (!Array.Exists(validExtensions, ext => ext == fileExtension))
                return false;

            var mimeType = avatarFile.ContentType.ToLower();
            if (!mimeType.StartsWith("image/"))
                return false;

            return true;
        }
    }
}
