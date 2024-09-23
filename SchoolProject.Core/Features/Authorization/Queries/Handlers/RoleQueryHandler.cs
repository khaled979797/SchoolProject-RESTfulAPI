using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Localization;
using SchoolProject.Core.Bases;
using SchoolProject.Core.Features.Authorization.Queries.Models;
using SchoolProject.Core.Features.Authorization.Queries.Responses;
using SchoolProject.Core.Resources;
using SchoolProject.Data.Entities.Identity;
using SchoolProject.Data.Responses;
using SchoolProject.Service.Abstracts;

namespace SchoolProject.Core.Features.Authorization.Queries.Handlers
{
    public class RoleQueryHandler : ResponseHandler,
        IRequestHandler<GetRolesListQuery, Response<List<GetRolesListResponse>>>,
        IRequestHandler<GetRoleByIdQuery, Response<GetRoleByIdResponse>>,
        IRequestHandler<ManageUserRolesQuery, Response<ManageUserRolesResponse>>
    {
        #region Fields
        private readonly IStringLocalizer<SharedResources> stringLocalizer;
        private readonly IAuthorizationService authorizationService;
        private readonly UserManager<User> userManager;
        private readonly IMapper mapper;

        #endregion

        #region Constructor
        public RoleQueryHandler(IStringLocalizer<SharedResources> stringLocalizer, IMapper mapper,
            IAuthorizationService authorizationService, UserManager<User> userManager) : base(stringLocalizer)
        {
            this.stringLocalizer = stringLocalizer;
            this.authorizationService = authorizationService;
            this.userManager = userManager;
            this.mapper = mapper;
        }
        #endregion

        #region Functions
        public async Task<Response<List<GetRolesListResponse>>> Handle(GetRolesListQuery request, CancellationToken cancellationToken)
        {
            var roles = await authorizationService.GetRolesList();
            var rolesMapper = mapper.Map<List<GetRolesListResponse>>(roles);
            return Success(rolesMapper);
        }

        public async Task<Response<GetRoleByIdResponse>> Handle(GetRoleByIdQuery request, CancellationToken cancellationToken)
        {
            var role = await authorizationService.GetRoleById(request.Id);
            if (role == null) return NotFound<GetRoleByIdResponse>(stringLocalizer[SharedResourcesKeys.RoleNotExist]);
            var roleMapper = mapper.Map<GetRoleByIdResponse>(role);
            return Success(roleMapper);
        }

        public async Task<Response<ManageUserRolesResponse>> Handle(ManageUserRolesQuery request, CancellationToken cancellationToken)
        {
            var user = await userManager.FindByIdAsync(request.UserId.ToString());
            if (user == null) return NotFound<ManageUserRolesResponse>(stringLocalizer[SharedResourcesKeys.UserIsNotFound]);
            var result = await authorizationService.GetManageUserRolesData(user);
            return Success(result);
        }
        #endregion
    }
}
