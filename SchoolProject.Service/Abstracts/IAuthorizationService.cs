using SchoolProject.Data.Requests;

namespace SchoolProject.Service.Abstracts
{
    public interface IAuthorizationService
    {
        public Task<string> AddRoleAsync(string roleName);
        public Task<bool> IsRoleExistByName(string roleName);
        public Task<string> EditRoleAsync(EditRoleRequest editRoleRequest);
        public Task<string> DeleteRoleAsync(int roleId);
    }
}