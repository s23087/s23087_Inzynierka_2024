using System.Security.Cryptography;
using System.Text;

namespace database_comunicator.Utils
{
    public class Hasher
    {
        public static bool VerifyPassword(string hash, string salt, string password)
        {
            string hashToVerify = CreateHashPassword(password, salt);
            return hash.Equals(hashToVerify);
        }
        public static string CreateHashPassword(string password, string salt)
        {
            byte[] passToBytes = Encoding.UTF8.GetBytes(password);
            byte[] saltBytes = Convert.FromBase64String(salt);

            byte[] combined = new byte[passToBytes.Length + saltBytes.Length];
            Array.Copy(passToBytes, 0, combined, 0, passToBytes.Length);
            Array.Copy(saltBytes, 0, combined, passToBytes.Length, saltBytes.Length);

            byte[] hash = SHA256.HashData(combined);
            return Convert.ToBase64String(hash);
        }
        public static string GenerateSalt()
        {
            byte[] salt = new byte[16];
            using var random = RandomNumberGenerator.Create();
            random.GetBytes(salt);
            return Convert.ToBase64String(salt);
        }
    }
}
