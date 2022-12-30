using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace ViaChatServer.BuildingBlocks.Infrastructure.Utilities
{
    public static class HashUtils
    {
        public static string GenerateMD5(string value)
        {
            var algo = MD5.Create();

            return GenerateHashString(algo, value);
        }

        public static string GenerateSHA1(string value)
        {
            var algo = SHA1.Create();

            return GenerateHashString(algo, value);
        }

        public static string GenerateSHA256(string value)
        {
            var algo = SHA256.Create();

            return GenerateHashString(algo, value);
        }
        private static string GenerateHashString(HashAlgorithm algo, string value)
        {
            // Compute hash from value parameter
            algo.ComputeHash(Encoding.UTF8.GetBytes(value));

            // Get has value in array of bytes
            // Return as hexadecimal string
            return string.Join(string.Empty, algo.Hash.Select(x => $"{x:x2}"));
        }
    }
}
