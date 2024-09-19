using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using SchoolProject.Data.Entities.Identity;
using SchoolProject.Data.Helpers;
using SchoolProject.Infrastructure.Abstracts;
using SchoolProject.Service.Abstracts;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace SchoolProject.Service.Implementations
{
    public class AuthenticationService : IAuthenticationService
    {
        #region Fields
        private readonly JwtSettings jwtSettings;
        private readonly IRefreshTokenRepository refreshTokenRepository;
        private readonly UserManager<User> userManager;
        #endregion

        #region Constructor
        public AuthenticationService(JwtSettings jwtSettings,
            IRefreshTokenRepository refreshTokenRepository, UserManager<User> userManager)
        {
            this.jwtSettings = jwtSettings;
            this.refreshTokenRepository = refreshTokenRepository;
            this.userManager = userManager;
        }
        #endregion

        #region Functions
        private List<Claim> GetClaims(User user)
        {
            return new List<Claim>
            {
                new Claim(nameof(UserClaimModel.Id), user.Id.ToString()),
                new Claim(nameof(UserClaimModel.UserName), user.UserName),
                new Claim(nameof(UserClaimModel.Email), user.Email),
                new Claim(nameof(UserClaimModel.PhoneNumber), user.PhoneNumber)
            };
        }

        private SigningCredentials GetSigningCredentials()
        {
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings.Secret));
            return new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256Signature);
        }

        private (JwtSecurityToken, string) GenerateJwtToken(User user)
        {
            var jwtToken = new JwtSecurityToken
            (
                issuer: jwtSettings.Issuer,
                audience: jwtSettings.Audience,
                claims: GetClaims(user),
                expires: DateTime.Now.AddDays(jwtSettings.AccessTokenExpireDate),
                signingCredentials: GetSigningCredentials()
            );
            var accessToken = new JwtSecurityTokenHandler().WriteToken(jwtToken);
            return (jwtToken, accessToken);
        }

        private string GenerateRefreshToken()
        {
            var randomNumber = new byte[32];
            var randomNumberGenerater = RandomNumberGenerator.Create();
            randomNumberGenerater.GetBytes(randomNumber);
            return Convert.ToBase64String(randomNumber);
        }

        private RefreshToken GetRefreshToken(string userName)
        {
            return new RefreshToken
            {
                UserName = userName,
                TokenString = GenerateRefreshToken(),
                ExpireAt = DateTime.Now.AddDays(jwtSettings.RefreshTokenExpireDate),
            };
        }

        public JwtSecurityToken ReadJwtToken(string accessToken)
        {
            if (String.IsNullOrEmpty(accessToken)) throw new ArgumentNullException(nameof(accessToken));
            var handler = new JwtSecurityTokenHandler();
            return handler.ReadJwtToken(accessToken);
        }

        public async Task<JwtAuthResult> GetJwtToken(User user)
        {
            var (jwtToken, accessToken) = GenerateJwtToken(user);

            var userRefreshToken = new UserRefreshToken
            {
                UserId = user.Id,
                Token = accessToken,
                RefreshToken = GetRefreshToken(user.UserName).TokenString,
                JwtId = jwtToken.Id,
                IsUsed = true,
                IsRevoked = false,
                AddedTime = DateTime.Now,
                ExpireDate = DateTime.Now.AddDays(jwtSettings.RefreshTokenExpireDate)
            };
            await refreshTokenRepository.AddAsync(userRefreshToken);

            return new JwtAuthResult
            {
                AccessToken = accessToken,
                RefreshToken = GetRefreshToken(user.UserName)
            };
        }

        public async Task<JwtAuthResult> GetRefreshToken(User user, JwtSecurityToken jwtToken, string refreshToken, DateTime? expireDate)
        {
            var (jwtSecurityToken, newAccessToken) = GenerateJwtToken(user);

            var refreshTokenResult = new RefreshToken
            {
                UserName = jwtToken.Claims.FirstOrDefault(x => x.Type.Equals(nameof(UserClaimModel.UserName))).Value,
                TokenString = refreshToken,
                ExpireAt = Convert.ToDateTime(expireDate),
            };

            return new JwtAuthResult
            {
                AccessToken = newAccessToken,
                RefreshToken = refreshTokenResult
            };
        }

        public async Task<string> ValidateToken(string accessToken)
        {
            var handler = new JwtSecurityTokenHandler();
            var parameters = new TokenValidationParameters
            {
                ValidateIssuer = jwtSettings.ValidateIssuer,
                ValidIssuers = new[] { jwtSettings.Issuer },
                ValidateIssuerSigningKey = jwtSettings.ValidateIssuerSigningKey,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings.Secret)),
                ValidAudience = jwtSettings.Audience,
                ValidateAudience = jwtSettings.ValidateAudience,
                ValidateLifetime = jwtSettings.ValidateLifeTime,
            };

            try
            {
                var validator = handler.ValidateToken(accessToken, parameters, out SecurityToken validatedToken);
                if (validator == null) return "InvalidToken";
                return "NotExpired";
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }

        public async Task<(string, DateTime?)> ValidateDetails(JwtSecurityToken jwtToken, string accessToken, string refreshToken)
        {
            if (jwtToken == null || !jwtToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256Signature))
            {
                return ("AlgorithmIsWrong", null);
            }
            if (jwtToken.ValidTo > DateTime.UtcNow) return ("TokenIsNotExpired", null);

            //Get UserRefreshToken
            var userId = jwtToken.Claims.FirstOrDefault(x => x.Type.Equals(nameof(UserClaimModel.Id))).Value;
            var userRefreshToken = await refreshTokenRepository.GetTableNoTracking()
                .FirstOrDefaultAsync(x => x.Token == accessToken && x.RefreshToken == refreshToken
                && x.UserId == int.Parse(userId));

            //Validation RefreshToken
            if (userRefreshToken == null) return ("RefreshTokenIsNotFound", null);
            if (userRefreshToken.ExpireDate < DateTime.UtcNow)
            {
                userRefreshToken.IsRevoked = true;
                userRefreshToken.IsUsed = false;
                await refreshTokenRepository.UpdateAsync(userRefreshToken);
                return ("RefreshTokenIsExpired", null);
            }
            var expiredDate = userRefreshToken.ExpireDate;
            return (userId, expiredDate);
        }
        #endregion
    }
}
