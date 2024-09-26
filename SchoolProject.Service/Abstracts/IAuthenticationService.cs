using SchoolProject.Data.Entities.Identity;
using SchoolProject.Data.Responses;
using System.IdentityModel.Tokens.Jwt;

namespace SchoolProject.Service.Abstracts
{
    public interface IAuthenticationService
    {
        public Task<JwtAuthResponse> GetJwtToken(User user);
        public JwtSecurityToken ReadJwtToken(string accessToken);
        public Task<(string, DateTime?)> ValidateDetails(JwtSecurityToken jwtToken, string accessToken, string refreshToken);
        public Task<JwtAuthResponse> GetRefreshToken(User user, JwtSecurityToken jwtToken, string refreshToken, DateTime? expireDate);
        public Task<string> ValidateToken(string accessToken);
        public Task<string> ConfirmEmail(int? userId, string? code);
        public Task<string> SendResetPasswordCode(string email);
        public Task<string> ConfirmResetPassword(string code, string email);
        public Task<string> ResetPassword(string email, string password);
    }
}
