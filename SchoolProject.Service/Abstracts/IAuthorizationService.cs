using SchoolProject.Data.Entities.Identity;
using SchoolProject.Data.Requests;
using SchoolProject.Data.Responses;

namespace SchoolProject.Service.Abstracts
{
    public interface IAuthorizationService
    {
        public Task<string> AddRoleAsync(string roleName);
        public Task<bool> IsRoleExist(string roleName);
        public Task<string> EditRoleAsync(EditRoleRequest editRoleRequest);
        public Task<string> DeleteRoleAsync(int roleId);
        public Task<List<Role>> GetRolesList();
        public Task<Role> GetRoleById(int id);
        public Task<ManageUserRolesResponse> GetManageUserRolesData(User user);
        public Task<string> UpdateUserRoles(EditUserRolesRequest request);
    }
}