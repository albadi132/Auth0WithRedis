using System.Security.Cryptography;

namespace Auth0WithRedis.Utility
{
    public class RefreshToken
    {

        public static string Generate()
        {
            var randomNumber = new byte[64];
            using var rng = RandomNumberGenerator.Create();
            rng.GetBytes(randomNumber);
            return Convert.ToBase64String(randomNumber);
        }
    }
}
