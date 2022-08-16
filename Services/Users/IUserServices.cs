using Auth0WithRedis.Dto.Tokens;
using Auth0WithRedis.Dto.Users;
using Auth0WithRedis.Models;
using Auth0WithRedis.Models.Api;

namespace Auth0WithRedis.Services.Users
{
    public interface IUserServices
    {

        Task<ServiceResponse<AuthUsers>> CheckUserCredentials(UserCredentials Credentials);
        Task<bool> SetRefreshToken(string RefreshToken, AuthUsers AuthUsers);

        Task<ServiceResponse<UserWithToken>> GenerateTokenFromRefreshToken(string RefreshToken);
    }
}
