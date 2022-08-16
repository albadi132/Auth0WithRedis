using Auth0WithRedis.Dto.Users;
using Auth0WithRedis.Models;

namespace Auth0WithRedis.Dto.Tokens
{
    public class UserWithToken
    {

        public RefreshAndAccessToken RefreshAndAccessToken { get; set; }

        public AuthUsers AuthUsers { get; set; }
    }
}
