using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using SchoolProject.Data.Entities.Identity;
using SchoolProject.Data.Helpers;
using SchoolProject.Data.Responses;
using SchoolProject.Infrastructure.Abstracts;
using SchoolProject.Infrastructure.Data;
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
        private readonly IEmailService emailService;
        private readonly IRefreshTokenRepository refreshTokenRepository;
        private readonly UserManager<User> userManager;
        private readonly AppDbContext context;
        #endregion

        #region Constructor
        public AuthenticationService(JwtSettings jwtSettings,
            IRefreshTokenRepository refreshTokenRepository, UserManager<User> userManager,
            IEmailService emailService, AppDbContext context)
        {
            this.jwtSettings = jwtSettings;
            this.emailService = emailService;
            this.refreshTokenRepository = refreshTokenRepository;
            this.userManager = userManager;
            this.context = context;
        }
        #endregion

        #region Functions
        private async Task<List<Claim>> GetClaims(User user)
        {
            var claims = new List<Claim>
            {
                new Claim(nameof(UserClaimModel.Id), user.Id.ToString()),
                new Claim(ClaimTypes.Name, user.UserName),
                new Claim(ClaimTypes.NameIdentifier, user.UserName),
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(nameof(UserClaimModel.PhoneNumber), user.PhoneNumber)
            };

            var roles = await userManager.GetRolesAsync(user);
            foreach (var role in roles)
            {
                claims.Add(new Claim(ClaimTypes.Role, role));
            }

            var userClaims = await userManager.GetClaimsAsync(user);
            claims.AddRange(userClaims);

            return claims;
        }

        private SigningCredentials GetSigningCredentials()
        {
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings.Secret));
            return new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256Signature);
        }

        private async Task<(JwtSecurityToken, string)> GenerateJwtToken(User user)
        {
            var jwtToken = new JwtSecurityToken
            (
                issuer: jwtSettings.Issuer,
                audience: jwtSettings.Audience,
                claims: await GetClaims(user),
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

        public async Task<JwtAuthResponse> GetJwtToken(User user)
        {
            var (jwtToken, accessToken) = await GenerateJwtToken(user);

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

            return new JwtAuthResponse
            {
                AccessToken = accessToken,
                RefreshToken = GetRefreshToken(user.UserName)
            };
        }

        public async Task<JwtAuthResponse> GetRefreshToken(User user, JwtSecurityToken jwtToken, string refreshToken, DateTime? expireDate)
        {
            var (jwtSecurityToken, newAccessToken) = await GenerateJwtToken(user);

            var refreshTokenResult = new RefreshToken
            {
                UserName = jwtToken.Claims.FirstOrDefault(x => x.Type.Equals(nameof(UserClaimModel.UserName))).Value,
                TokenString = refreshToken,
                ExpireAt = Convert.ToDateTime(expireDate),
            };

            return new JwtAuthResponse
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

        public async Task<string> ConfirmEmail(int? userId, string? code)
        {
            if (userId == null || code == null) return "ErrorWhenConfirmEmail";

            var user = await userManager.FindByIdAsync(userId.ToString());
            var confirmEmail = await userManager.ConfirmEmailAsync(user, code);
            if (!confirmEmail.Succeeded) return "ErrorWhenConfirmEmail";
            return "Success";
        }

        public async Task<string> SendResetPasswordCode(string email)
        {
            var transact = await context.Database.BeginTransactionAsync();
            try
            {
                var user = await userManager.FindByEmailAsync(email);
                if (user == null) return "UserNotFound";

                Random generator = new Random();
                var randomNumber = generator.Next(0, 1000000).ToString("D6");
                user.Code = randomNumber;
                var updateResult = await userManager.UpdateAsync(user);
                if (!updateResult.Succeeded) return "ErrorInUpdateUser";

                var message = $"Code To Reset Password: {randomNumber}";
                await emailService.SendEmail(email, message, "Reset Password");
                await transact.CommitAsync();
                return "Success";
            }
            catch
            {
                await transact.RollbackAsync();
                return "Failed";
            }
        }

        public async Task<string> ConfirmResetPassword(string code, string email)
        {
            var user = await userManager.FindByEmailAsync(email);
            if (user == null) return "UserNotFound";

            var userCode = user.Code;
            if (userCode == code) return "Success";
            return "Failed";
        }

        public async Task<string> ResetPassword(string email, string password)
        {
            var transact = await context.Database.BeginTransactionAsync();
            try
            {
                var user = await userManager.FindByEmailAsync(email);
                if (user == null) return "UserNotFound";

                await userManager.RemovePasswordAsync(user);
                if (!await userManager.HasPasswordAsync(user))
                {
                    await userManager.AddPasswordAsync(user, password);
                }
                await transact.CommitAsync();
                return "Success";
            }
            catch
            {
                await transact.RollbackAsync();
                return "Failed";
            }
        }
        #endregion
    }
}
