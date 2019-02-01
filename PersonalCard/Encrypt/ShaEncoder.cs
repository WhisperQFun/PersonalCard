using System.Threading.Tasks;
using System.Security.Cryptography;
using System.Text;

namespace PersonalCard.Encrypt
{
    public static class ShaEncoder
    {
        public static async Task<string> GenerateSHA256String(string inputString)
        {
            SHA256 sha256 = SHA256.Create();
            byte[] bytes = Encoding.UTF8.GetBytes(inputString);
            byte[] hash = sha256.ComputeHash(bytes);
            return await GetStringFromHash(hash);
        }

        public static async Task<string> GenerateSHA512String(string inputString)
        {
            SHA512 sha512 = SHA512.Create();
            byte[] bytes = Encoding.UTF8.GetBytes(inputString);
            byte[] hash = sha512.ComputeHash(bytes);
            return await GetStringFromHash(hash);
        }

        private static async Task<string> GetStringFromHash(byte[] hash)
        {
            StringBuilder result = new StringBuilder();
            for (int i = 0; i < hash.Length; i++)
                result.Append(hash[i].ToString("X2"));

            return result.ToString();
        }
    }
}
