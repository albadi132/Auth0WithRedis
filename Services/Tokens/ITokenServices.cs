using Auth0WithRedis.Dto.Tokens;
using Auth0WithRedis.Models;
using Auth0WithRedis.Models.Api;

namespace Auth0WithRedis.Services.Tokens
{
    public interface ITokenServices
    {

        Task<ServiceResponse<RefreshAndAccessToken>> GenerateToken(AuthUsers AuthUsers);

       // Task<bool> SetRefreshToken(string RefreshToken, AuthUsers AuthUsers);


    }
}
