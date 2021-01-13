using System.Threading.Tasks;
using WebCore.Infrastructure.BusinessObjects;
using WebCore.ViewModels;

namespace WebCore.Services
{
    public interface IAuthenticationAppService
    {
        Task<GenericResult<LoginResponseModel>> AuthenticateWithUserNamePassword(LoginRequestModel model);
        Task<GenericResult<LoginResponseModel>> AuthenticateWithRefreshToken(RefreshTokenRequestModel model);
        Task<GenericResult<bool>> Logout(string token);
    }
}