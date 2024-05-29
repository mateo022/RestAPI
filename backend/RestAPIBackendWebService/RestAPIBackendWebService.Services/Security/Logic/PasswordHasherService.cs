using RestAPIBackendWebService.Services.Security.Contracts;
using Microsoft.Extensions.Configuration;
using System.Security.Cryptography;
using System.Text;

namespace RestAPIBackendWebService.Services.Security.Logic
{
    public class PasswordHasherService : IPasswordHasherService
    {
        private readonly int keySize;
        private readonly int iterations;
        private readonly HashAlgorithmName hashAlgorithm = HashAlgorithmName.SHA512;

        private readonly IConfiguration _configuration;

        public PasswordHasherService(IConfiguration configuration)
        {
            _configuration = configuration;

            keySize = Convert.ToInt32(_configuration["Hash:KeySize"]);
            iterations = Convert.ToInt32(_configuration["Hash:Iterations"]);
        }

        public string HashPassword(string password, out byte[] salt)
        {
            salt = RandomNumberGenerator.GetBytes(keySize);

            var hash = Rfc2898DeriveBytes.Pbkdf2(
                Encoding.UTF8.GetBytes(password),
                salt,
                iterations,
                hashAlgorithm,
                keySize);

            return Convert.ToHexString(hash);
        }

        public bool VerifyPassword(string password, string hash, byte[] salt)
        {
            var hashToCompare = Rfc2898DeriveBytes.Pbkdf2(password, salt, iterations, hashAlgorithm, keySize);
            return hashToCompare.SequenceEqual(Convert.FromHexString(hash));
        }

        public string HashMessageCode(string code, byte[] salt)
        {
            var hashCode = Rfc2898DeriveBytes.Pbkdf2(
                Encoding.UTF8.GetBytes(code),
                salt,
                iterations,
                hashAlgorithm,
                keySize);

            return Convert.ToHexString(hashCode);
        }
    }
}
