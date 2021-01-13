using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebCore.Infrastructure.BusinessObjects;
using WebCore.ViewModels;
using IAuthenticationAppService = WebCore.Services.IAuthenticationAppService;

namespace WebCore.Controllers
{
    [Route("api/Authentication")]
    public class AuthenticationController : Controller
    {
        // GET
        private readonly IAuthenticationAppService _authenticationService;

        public AuthenticationController(IAuthenticationAppService authenticationService)
        {
            _authenticationService = authenticationService;
        }

        [HttpGet("v1.0.0/login")]
        public async Task<GenericResult<LoginResponseModel>> Login()
        {
            var response = await _authenticationService.AuthenticateWithUserNamePassword(null);
            return response;
        }

        [HttpPost("v1.0.0/refresh-login")]
        public async Task<GenericResult<LoginResponseModel>> RefreshToken([FromBody] RefreshTokenRequestModel model)
        {
            var response = await _authenticationService.AuthenticateWithRefreshToken(model);
            return response;
        }

        [Authorize]
        [HttpDelete("v1.0.0/logout")]
        public async Task<GenericResult<bool>> Logout()
        {
            var accessToken = await HttpContext.GetTokenAsync("access_token");
            var response = await _authenticationService.Logout(accessToken);
            return response;
        }
    }
}