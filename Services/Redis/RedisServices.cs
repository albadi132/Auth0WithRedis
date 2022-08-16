using Auth0WithRedis.Dto.Tokens;
using Auth0WithRedis.Models;
using Auth0WithRedis.Models.Api;
using Auth0WithRedis.Services.Tokens;
using Auth0WithRedis.Services.Users;
using Microsoft.Extensions.Caching.Distributed;
using System.Text;
using System.Text.Json;

namespace Auth0WithRedis.Services.Redis
{
    public class RedisServices : IRedisServices
    {

        private readonly IDistributedCache _cache;
        ITokenServices _TokenServices;
        


        public RedisServices(IDistributedCache cache, ITokenServices tokenServices )
        {
            _cache = cache;
            _TokenServices = tokenServices;
        }


        public async Task<bool> SetRefreshToken(string RefreshToken , AuthUsers AuthUsers)
        {
          

            try
            {
                // Serializing the data
                string cachedDataString = JsonSerializer.Serialize(AuthUsers);
                var dataToCache = Encoding.UTF8.GetBytes(cachedDataString);

                // Setting up the cache options
                DistributedCacheEntryOptions options = new DistributedCacheEntryOptions()
                    .SetAbsoluteExpiration(DateTime.Now.AddMinutes(30))
                    .SetSlidingExpiration(TimeSpan.FromMinutes(30));

                // Add the data into the cache
                await _cache.SetAsync(RefreshToken, dataToCache, options);

            }
            catch (Exception exception)
            {
                //DO some loggin

                return false;
            }

           

            return true;
        }

        public async Task<ServiceResponse<UserWithToken>> GenerateTokenFromRefreshToken(string RefreshToken)
        {
            var serviceResponse = new ServiceResponse<UserWithToken>();

            try
            {
                // Trying to get data from the Redis cache
                byte[] cachedData = await _cache.GetAsync(RefreshToken);
                AuthUsers User = new();
                if (cachedData != null)
                {
                    // If the data is found in the cache, encode and deserialize cached data.
                    var cachedDataString = Encoding.UTF8.GetString(cachedData);
                    User = JsonSerializer.Deserialize<AuthUsers>(cachedDataString);


                    //Genrate access token &  refresh token
                    var Token = await _TokenServices.GenerateToken(User);
                    if (Token.Success)
                    {
                        serviceResponse.Data = new UserWithToken();
                        serviceResponse.Data.RefreshAndAccessToken = Token.Data;
                        serviceResponse.Data.AuthUsers = User;
                        serviceResponse.Success = true;
                        serviceResponse.Code = 200;
                        serviceResponse.Meesage = "Success";

                        return serviceResponse;

                    }
                    
                }

                

            }
            catch (Exception exception)
            {
                serviceResponse.Success = false;
                serviceResponse.Meesage = "Something went wrong!";
                serviceResponse.Code = 500;
            }



            return serviceResponse;
        }

    }
}
