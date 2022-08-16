using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Net.Http.Headers;
using System.IdentityModel.Tokens.Jwt;

namespace Auth0WithRedis.Controllers
{
    [Authorize]
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class UserController : Controller
    {
       
        [HttpGet]
        public IActionResult SayMyName()
        {
            var _bearer_token = Request.Headers[HeaderNames.Authorization].ToString().Replace("Bearer ", "");

            var handler = new JwtSecurityTokenHandler();
            var decodedValue = handler.ReadJwtToken(_bearer_token);

            return Ok(decodedValue.Claims.FirstOrDefault(c => c.Type == "DisplayName").Value);
        }
    }
}
