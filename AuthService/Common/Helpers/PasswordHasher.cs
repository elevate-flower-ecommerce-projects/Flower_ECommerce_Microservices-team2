using System.Security.Cryptography;

namespace AuthService.Common.Helpers
{
    public static class PasswordHasher
    {
        private const int SaltSize = 16; // 128-bit
        private const int KeySize = 32;  // 256-bit
        private const int Iterations = 100000;
        private static readonly HashAlgorithmName HashAlgorithm = HashAlgorithmName.SHA256;
        public static string Hash(string password)
        {
            byte[] salt = RandomNumberGenerator.GetBytes(SaltSize);
            byte[] hash = Rfc2898DeriveBytes.Pbkdf2(password, salt, Iterations, HashAlgorithm, KeySize);
            return $"{Convert.ToBase64String(salt)}.{Convert.ToBase64String(hash)}";
        }
        public static bool Verify(string password, string hashedPassword)
        {
            string[] parts = hashedPassword.Split('.', 2);
            if (parts.Length != 2) return false;
            byte[] salt = Convert.FromBase64String(parts[0]);
            byte[] hash = Convert.FromBase64String(parts[1]);
            byte[] inputHash = Rfc2898DeriveBytes.Pbkdf2(password, salt, Iterations, HashAlgorithm, KeySize);
            return CryptographicOperations.FixedTimeEquals(hash, inputHash);
        }
    }
}
