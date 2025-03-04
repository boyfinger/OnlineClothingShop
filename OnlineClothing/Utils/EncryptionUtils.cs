using System.Security.Cryptography;
using System.Text;

namespace OnlineClothing.Utils
{
    public class EncryptionUtils
    {
        // Method to encrypt with SHA-256
        public static string EncodeSha256(string input)
        {
            // Create a new SHA256 instance
            using (SHA256 sha256Hash = SHA256.Create())
            {
                // Compute hash bytes from the input string
                byte[] bytes = sha256Hash.ComputeHash(Encoding.UTF8.GetBytes(input));

                // Convert byte array to a hexadecimal string
                StringBuilder builder = new StringBuilder();
                foreach (byte b in bytes)
                {
                    builder.Append(b.ToString("x2"));
                }
                // Return the resulting SHA-256 hash as a string
                return builder.ToString();
            }
        }
    }
}
