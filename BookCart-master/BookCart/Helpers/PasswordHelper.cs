using System.Security.Cryptography;
using System.Text;

namespace BookCart.Helpers
{
    public static class PasswordHelper
    {
        public static string HashPassword(string password)
        {
            using var sha256 = SHA256.Create();
            var hashedBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
            return BitConverter.ToString(hashedBytes).Replace("-", "").ToLower();
        }

        public static bool VerifyPassword(string inputPassword, string storedHash)
        {
            // Handle both MySQL SHA2 and C# SHA256 formats
            var hashOfInput = HashPassword(inputPassword);
            return hashOfInput.Equals(storedHash, StringComparison.OrdinalIgnoreCase);
        }
    }
}
