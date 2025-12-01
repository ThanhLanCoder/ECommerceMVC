using System.Security.Cryptography;
using System.Text;

namespace ECommerceMVC.Helper
{
    public class PasswordHasher
    {
        public static string HashPassword(string password)
        {
            var sha256 = SHA256.Create();
            byte[] bytes = Encoding.UTF8.GetBytes(password);
            byte[] hash = sha256.ComputeHash(bytes);

            StringBuilder sb = new StringBuilder();
            foreach (byte b in hash)
                sb.Append(b.ToString("x2"));  // mỗi byte 2 ký tự hex

            return sb.ToString();
        }
    }
}

