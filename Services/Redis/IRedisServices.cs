using Auth0WithRedis.Dto.Tokens;
using Auth0WithRedis.Models;
using Auth0WithRedis.Models.Api;

namespace Auth0WithRedis.Services.Redis
{
    public interface IRedisServices
    {

        
            Task<bool> SetRefreshToken(string RefreshToken, AuthUsers AuthUsers);

            Task<ServiceResponse<UserWithToken>> GenerateTokenFromRefreshToken(string RefreshToken );


    }
}
