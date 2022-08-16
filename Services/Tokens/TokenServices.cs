using Auth0WithRedis.Data;
using Auth0WithRedis.Dto.Tokens;
using Auth0WithRedis.Models;
using Auth0WithRedis.Models.Api;
using Auth0WithRedis.Services.Redis;
using Auth0WithRedis.Services.Users;
using Auth0WithRedis.Utility;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace Auth0WithRedis.Services.Tokens
{
    public class TokenServices : ITokenServices
    {
        public IConfiguration _configuration;

        public TokenServices(IConfiguration config)
        {
            _configuration = config;
            
        }



        public async Task<ServiceResponse<RefreshAndAccessToken>> GenerateToken(AuthUsers AuthUsers)
        {
            var serviceResponse = new ServiceResponse<RefreshAndAccessToken>();
           

            try
            {

                //create claims details based on the user information
                var claims = new[] {
                        new Claim(JwtRegisteredClaimNames.Sub, _configuration["Jwt:Subject"]),
                        new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                        new Claim(JwtRegisteredClaimNames.Iat, DateTime.UtcNow.ToString()),
                        new Claim("UserId", AuthUsers.Id.ToString()),
                        new Claim("DisplayName", AuthUsers.FirstName + " " + AuthUsers.LastName),
                        new Claim("Email", AuthUsers.Email)
                    };

                var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));
                var signIn = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
                var token = new JwtSecurityToken(
                    _configuration["Jwt:Issuer"],
                    _configuration["Jwt:Audience"],
                    claims,
                    expires: DateTime.UtcNow.AddMinutes(10),
                    signingCredentials: signIn);

                serviceResponse.Data = new RefreshAndAccessToken();
                serviceResponse.Data.AccessToken = new JwtSecurityTokenHandler().WriteToken(token);
                serviceResponse.Data.RefreshToken = RefreshToken.Generate();
                serviceResponse.Success = true;
                serviceResponse.Code = 200;
                serviceResponse.Meesage = "Success";


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
