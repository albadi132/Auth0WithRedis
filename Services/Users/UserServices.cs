using Auth0WithRedis.Data;
using Auth0WithRedis.Dto.Tokens;
using Auth0WithRedis.Dto.Users;
using Auth0WithRedis.Models;
using Auth0WithRedis.Models.Api;
using Auth0WithRedis.Services.Redis;
using Auth0WithRedis.Services.Tokens;
using Auth0WithRedis.Utility;
using Microsoft.EntityFrameworkCore;

namespace Auth0WithRedis.Services.Users
{
    public class UserServices : IUserServices
    {
        private readonly DimoDBContext _db;
        ITokenServices _TokenServices;

        public UserServices(DimoDBContext db , ITokenServices TokenServices )
        {
            _db = db;
            _TokenServices = TokenServices;
        }

        public async Task<ServiceResponse<AuthUsers>> CheckUserCredentials(UserCredentials Credentials)
        {
            var serviceResponse = new ServiceResponse<AuthUsers>();
            

            try
            {
                var User = await _db.AuthUsers.Where(s => s.Email == Credentials.Email && s.Password == HashPassword.Sha256(Credentials.Password) ).FirstOrDefaultAsync();

                if (User == null) return serviceResponse;

                serviceResponse.Data = new AuthUsers();
                serviceResponse.Data = User;
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

        public async Task<bool> SetRefreshToken(string RefreshToken, AuthUsers AuthUsers)
        {
           
            try
            {
                _db.Add(new AuthLogin { AuthUserId = AuthUsers.Id, RefreshToken = RefreshToken });
                _db.SaveChanges();

            }
            catch (Exception exception)
            {
                return false;
            }

            return true;
        }

        public async Task<ServiceResponse<UserWithToken>> GenerateTokenFromRefreshToken(string RefreshToken)
        {
            var serviceResponse = new ServiceResponse<UserWithToken>();

            try
            {
                var LoginUser = await _db.AuthLogin
                    .Include(s => s.AuthUser)
                    .Where(s => s.RefreshToken == RefreshToken).FirstAsync();

                if (LoginUser != null)
                {

                    //Genrate access token &  refresh token
                    var Token = await _TokenServices.GenerateToken(LoginUser.AuthUser);
                    if (Token.Success)
                    {
                        serviceResponse.Data = new UserWithToken();
                        serviceResponse.Data.RefreshAndAccessToken = Token.Data;
                        serviceResponse.Data.AuthUsers = LoginUser.AuthUser;
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
