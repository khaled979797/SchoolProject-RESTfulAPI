using Microsoft.IdentityModel.Tokens;
using SchoolProject.Data.Entities.Identity;
using SchoolProject.Data.Helpers;
using SchoolProject.Infrastructure.Abstracts;
using SchoolProject.Service.Abstracts;
using System.Collections.Concurrent;
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
        private readonly ConcurrentDictionary<string, RefreshToken> userRefreshToken;
        #endregion

        #region Constructor
        public AuthenticationService(JwtSettings jwtSettings, IRefreshTokenRepository refreshTokenRepository)
        {
            this.jwtSettings = jwtSettings;
            this.refreshTokenRepository = refreshTokenRepository;
            userRefreshToken = new ConcurrentDictionary<string, RefreshToken>();
        }
        #endregion

        #region Functions
        private List<Claim> GetClaims(User user)
        {
            return new List<Claim>
            {
                new Claim(nameof(UserClaimModel.UserName), user.UserName),
                new Claim(nameof(UserClaimModel.Email), user.Email),
                new Claim(nameof(UserClaimModel.PhoneNumber), user.PhoneNumber),
            };
        }

        private SigningCredentials GetSigningCredentials()
        {
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings.Secret));
            return new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);
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
            var refreshToken = new RefreshToken
            {
                UserName = userName,
                TokenString = GenerateRefreshToken(),
                ExpireAt = DateTime.Now.AddDays(jwtSettings.RefreshTokenExpireDate),
            };

            userRefreshToken.AddOrUpdate(refreshToken.TokenString, refreshToken, (t, s) => refreshToken);
            return refreshToken;
        }

        public async Task<JwtAuthResult> GetJwtToken(User user)
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

            var userRefreshToken = new UserRefreshToken
            {
                UserId = user.Id,
                Token = accessToken,
                RefreshToken = GetRefreshToken(user.UserName).TokenString,
                JwtId = jwtToken.Id,
                IsUsed = false,
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
        #endregion
    }
}
