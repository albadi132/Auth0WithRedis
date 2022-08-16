using System.Security.Cryptography;
using System.Text;

namespace Auth0WithRedis.Utility
{
    public class HashPassword
    {


        public static string Sha256(string password)
        {

            // SHA512 is disposable by inheritance.  
            var sha256 = SHA256.Create();
            // Send a sample text to hash.  
            byte[] varhashedBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));

            return BitConverter.ToString(varhashedBytes).Replace("-", String.Empty);

        }
    }
}
