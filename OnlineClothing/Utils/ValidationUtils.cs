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
            return gender == "Male" || gender == "Female" || gender == "Other";
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
    }
}
