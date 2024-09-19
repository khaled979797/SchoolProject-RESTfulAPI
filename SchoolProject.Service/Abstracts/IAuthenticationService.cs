using SchoolProject.Data.Entities.Identity;
using SchoolProject.Data.Helpers;
using System.IdentityModel.Tokens.Jwt;

namespace SchoolProject.Service.Abstracts
{
    public interface IAuthenticationService
    {
        public Task<JwtAuthResult> GetJwtToken(User user);
        public JwtSecurityToken ReadJwtToken(string accessToken);
        public Task<(string, DateTime?)> ValidateDetails(JwtSecurityToken jwtToken, string accessToken, string refreshToken);
        public Task<JwtAuthResult> GetRefreshToken(User user, JwtSecurityToken jwtToken, string refreshToken, DateTime? expireDate);
        public Task<string> ValidateToken(string accessToken);
    }
}
