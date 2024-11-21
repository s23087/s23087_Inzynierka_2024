using System.Security.Cryptography;
using System.Text;

namespace database_communicator.Utils
{
    /// <summary>
    /// This class has static function that allow to encrypt, decrypt and verify password
    /// </summary>
    public static class Hasher
    {
        /// <summary>
        /// Verify given password.
        /// </summary>
        /// <param name="hash">Hash from database</param>
        /// <param name="salt">Salt from database</param>
        /// <param name="password">Password to verify</param>
        /// <returns>True if successful or false if failure.</returns>
        public static bool VerifyPassword(string hash, string salt, string password)
        {
            string hashToVerify = CreateHashPassword(password, salt);
            return hash.Equals(hashToVerify);
        }
        /// <summary>
        /// Create hashed version of given password using salt and SHA256 algorithm.
        /// </summary>
        /// <param name="password">String to encrypt</param>
        /// <param name="salt">Salt value</param>
        /// <returns>Encrypted password</returns>
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
        /// <summary>
        /// Generate salt for password.
        /// </summary>
        /// <returns>String that contain salt.</returns>
        public static string GenerateSalt()
        {
            byte[] salt = new byte[16];
            using var random = RandomNumberGenerator.Create();
            random.GetBytes(salt);
            return Convert.ToBase64String(salt);
        }
    }
}
