using Auth0WithRedis.Dto.Users;
using Auth0WithRedis.Models;
using Auth0WithRedis.Services.Redis;
using Auth0WithRedis.Services.Tokens;
using Auth0WithRedis.Services.Users;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Distributed;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;

namespace Auth0WithRedis.Controllers
{

    [Route("api/[controller]/[action]")]
    [ApiController]
    public class AuthController : Controller
    {

        IUserServices _UserServices;
        ITokenServices _TokenServices;
        IRedisServices _RedisServices;
        private readonly IDistributedCache _cache;

        public AuthController( IUserServices UserServices , ITokenServices TokenServices , IDistributedCache cache , IRedisServices RedisServices)
        {
            _UserServices = UserServices;
            _TokenServices = TokenServices;
            _RedisServices = RedisServices;
            _cache = cache; 

        }

        [HttpPost]
        [Route("UserLogin")]
        public async Task<IActionResult> Login(UserCredentials UserCredentials)
        {

            //check for user Credentials

           var User =  await _UserServices.CheckUserCredentials(UserCredentials);
            
            if(User.Success)
            {
                //Genrate access token &  refresh token
                var Token = await _TokenServices.GenerateToken(User.Data);
                if(Token.Success)
                {

                    _RedisServices.SetRefreshToken(Token.Data.RefreshToken, User.Data);
                    _UserServices.SetRefreshToken(Token.Data.RefreshToken, User.Data);

                    return Ok(Token);
                }
                
            }

            return Ok(User);
        }


        [HttpPost]
        public async Task<IActionResult> GenerateAccessTokenWithRedis([FromBody] string RefreshToken)
        {
            var TokenWithUser = await _RedisServices.GenerateTokenFromRefreshToken(RefreshToken);

            if(TokenWithUser.Success)
            {
                _RedisServices.SetRefreshToken(TokenWithUser.Data.RefreshAndAccessToken.RefreshToken, TokenWithUser.Data.AuthUsers);
                _UserServices.SetRefreshToken(TokenWithUser.Data.RefreshAndAccessToken.RefreshToken, TokenWithUser.Data.AuthUsers);

                return Ok(TokenWithUser.Data.RefreshAndAccessToken);

            }

            return Ok("need to login");

        }

        [HttpPost]
        public async Task<IActionResult> GenerateAccessTokenWithDB([FromBody] string RefreshToken)
        {
           var TokenWithUser = await _UserServices.GenerateTokenFromRefreshToken(RefreshToken);

            if (TokenWithUser.Success)
            {
                _RedisServices.SetRefreshToken(TokenWithUser.Data.RefreshAndAccessToken.RefreshToken, TokenWithUser.Data.AuthUsers);
                _UserServices.SetRefreshToken(TokenWithUser.Data.RefreshAndAccessToken.RefreshToken, TokenWithUser.Data.AuthUsers);

                return Ok(TokenWithUser.Data.RefreshAndAccessToken);

            }

            return Ok("need to login");

        }


        [HttpPost]
        [Route("GetHash")]
        public IActionResult GetHash(string password)
        {

            // SHA512 is disposable by inheritance.  
            var sha256 = SHA256.Create();
            // Send a sample text to hash.  
            byte[] varhashedBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));

            return Ok(BitConverter.ToString(varhashedBytes).Replace("-", String.Empty));

        }






    }
}
