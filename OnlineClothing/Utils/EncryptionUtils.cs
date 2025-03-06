using System.Security.Cryptography;
using System.Text;

namespace OnlineClothing.Utils
{
    public class EncryptionUtils
    {
        //Hash password with sha256
        public static string EncodeSha256(string input)
        {
            using (SHA256 sha256Hash = SHA256.Create())
            {
                byte[] bytes = sha256Hash.ComputeHash(Encoding.UTF8.GetBytes(input));

                StringBuilder builder = new StringBuilder();
                foreach (byte b in bytes)
                {
                    builder.Append(b.ToString("x2"));
                }
                return builder.ToString();
            }
        }


    }
}
