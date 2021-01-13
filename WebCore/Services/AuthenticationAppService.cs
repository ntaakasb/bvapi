using System;
using System.Collections.Generic;
using System.Globalization;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using WebCore.Constants;
using WebCore.Infrastructure.BusinessObjects;
using WebCore.Models;
using WebCore.Models.Table;
using WebCore.ViewModels;

namespace WebCore.Services
{
    public class AuthenticationAppService : IAuthenticationAppService
    {
        private readonly TokenOptions _tokenOptions;
        private readonly DwhContext _dwhContext;
        public AuthenticationAppService(IOptions<TokenOptions> tokenOptions, DwhContext dwhContext)
        {
            _dwhContext = dwhContext;
            _tokenOptions = tokenOptions.Value;
        }

        public async Task<GenericResult<LoginResponseModel>> AuthenticateWithUserNamePassword(LoginRequestModel model)
        {
            try
            {
                var usr = _dwhContext.Users.ToList();
                if (model == null)
                {
                    return GenericResult<LoginResponseModel>.Fail("Request is invalid.");
                }

                throw new NotImplementedException();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }

        public async Task<GenericResult<LoginResponseModel>> AuthenticateWithRefreshToken(RefreshTokenRequestModel model)
        {
            throw new System.NotImplementedException();
        }

        public async Task<GenericResult<bool>> Logout(string token)
        {
            throw new System.NotImplementedException();
        }

        private LoginResponseModel GenerateNewTokens(
            User user,
            IEnumerable<string> userPermissions,
            DateTime refreshExpiration,
            string dataPrivacyCacheKey = "")
        {
            var returnOutput = GenerateAccessTokens(user, userPermissions,dataPrivacyCacheKey);
            returnOutput.RefreshToken = GenerateRefreshTokens(user, refreshExpiration);
            return returnOutput;
        }

        private LoginResponseModel GenerateAccessTokens(
            User user,
            IEnumerable<string> userPermissions,
            string dataPrivacyCacheKey = "")
        {
            return GenerateAccessTokens(user, userPermissions, TimeSpan.FromMinutes(_tokenOptions.AccessExpiration), dataPrivacyCacheKey);
        }

        private LoginResponseModel GenerateAccessTokens(
            User user,
            IEnumerable<string> userPermissions,
            TimeSpan liveTime,
            string positionDataPrivacyCacheKey = "")
        {
            var now = DateTime.UtcNow;
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, user.USERNAME),
                new Claim(ClaimTypes.NameIdentifier, user.ID.ToString()),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(JwtRegisteredClaimNames.Iat, now.ToString(CultureInfo.InvariantCulture), ClaimValueTypes.Integer64),
                new Claim(CustomClaimType.DataPrivacyCacheKey, positionDataPrivacyCacheKey),
            };

            if (userPermissions != null)
            {
                claims.AddRange(userPermissions.Select(r => new Claim(ClaimTypes.Role, r)));
            }

            var expires = now.Add(liveTime);
            var jwtToken = new JwtSecurityToken(
                _tokenOptions.Issuer,
                _tokenOptions.Audience,
                claims,
                expires: expires,
                signingCredentials: new SigningCredentials(
                    new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_tokenOptions.Secret)),
                    SecurityAlgorithms.HmacSha512));

            var handler = new JwtSecurityTokenHandler();
            return new LoginResponseModel
            {
                Token = handler.WriteToken(jwtToken),
                ExpiresIn = expires
            };
        }

        private string GenerateRefreshTokens(User user, DateTime refreshExpiration)
        {
            var now = DateTime.UtcNow;
            var claim = new[]
            {
                new Claim(ClaimTypes.Email, user.USERNAME),
                new Claim(ClaimTypes.NameIdentifier, user.ID.ToString()),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(JwtRegisteredClaimNames.Iat, now.ToString(CultureInfo.InvariantCulture), ClaimValueTypes.Integer64),
            };

            var jwtToken = new JwtSecurityToken(
                _tokenOptions.Issuer,
                _tokenOptions.Audience,
                claim,
                expires: refreshExpiration,
                signingCredentials: new SigningCredentials(
                    new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_tokenOptions.RefreshSecret)),
                    SecurityAlgorithms.HmacSha512));

            var handler = new JwtSecurityTokenHandler();
            return handler.WriteToken(jwtToken);
        }
    }
}