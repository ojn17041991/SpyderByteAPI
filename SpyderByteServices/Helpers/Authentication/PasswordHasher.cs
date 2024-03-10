using Konscious.Security.Cryptography;
using Microsoft.Extensions.Configuration;
using SpyderByteServices.Models.Authentication;
using System.Security.Cryptography;
using System.Text;

namespace SpyderByteServices.Helpers.Authentication
{
    public class PasswordHasher
    {
        private readonly IConfiguration configuration;

        public PasswordHasher(IConfiguration configuration)
        {
            this.configuration = configuration;
        }

        public HashData GenerateNewHash(string password)
        {
            // Get password and salt as byte array.
            byte[] passwordBytes = Encoding.UTF8.GetBytes(password);
            List<byte> saltBytes = new List<byte>();

            // Generate salt.
            byte[] tempSaltBytes = generateSalt();
            string salt = Convert.ToBase64String(tempSaltBytes);
            saltBytes.AddRange(tempSaltBytes);

            // Generate pepper.
            byte pepperByte = generatePepper();
            char pepper = (char)pepperByte;
            saltBytes.Add(pepperByte);

            // Generate hash.
            byte[] hashBytes = generateHash(passwordBytes, saltBytes.ToArray());
            string hash = Convert.ToBase64String(hashBytes);

            return new HashData
            {
                Hash = hash,
                Salt = salt,
                Pepper = pepper
            };
        }

        public bool IsPasswordValid(PasswordVerification passwordVerification)
        {
            byte[] passwordBytes = Encoding.UTF8.GetBytes(passwordVerification.Password);
            byte[] saltBytes = Convert.FromBase64String(passwordVerification.Salt);

            for (int i = 0; i < 26; ++i)
            {
                byte pepperByte = (byte)(i + 97);
                List<byte> pepperedSaltBytes = new List<byte>();
                pepperedSaltBytes.AddRange(saltBytes);
                pepperedSaltBytes.Add(pepperByte);
                byte[] hashBytes = generateHash(passwordBytes, pepperedSaltBytes.ToArray());
                string hash = Convert.ToBase64String(hashBytes);

                if (hash == passwordVerification.Hash)
                {
                    return true;
                }
            }

            return false;
        }

        private byte[] generateSalt()
        {
            using (var rng = RandomNumberGenerator.Create())
            {
                byte[] bytes = new byte[32];
                rng.GetBytes(bytes);
                return bytes;
            }
        }

        private byte generatePepper()
        {
            return (byte)(new Random().Next(26) + 97);
        }

        private byte[] generateHash(byte[] password, byte[] salt)
        {
            Argon2 argon = new Argon2i(password);
            argon.DegreeOfParallelism = Convert.ToInt32(configuration["Encryption:Argon2:DegreesOfParallelism"]);
            argon.MemorySize = Convert.ToInt32(configuration["Encryption:Argon2:MemorySize"]);
            argon.Iterations = Convert.ToInt32(configuration["Encryption:Argon2:Iterations"]);
            argon.Salt = salt;
            return argon.GetBytes(32);
        }
    }
}
