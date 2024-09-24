using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Localization;
using SchoolProject.Core.Bases;
using SchoolProject.Core.Features.Authorization.Queries.Models;
using SchoolProject.Core.Resources;
using SchoolProject.Data.Entities.Identity;
using SchoolProject.Data.Responses;
using SchoolProject.Service.Abstracts;

namespace SchoolProject.Core.Features.Authorization.Queries.Handlers
{
    public class ClaimQueryHandler : ResponseHandler,
        IRequestHandler<ManageUserClaimsQuery, Response<ManagerUserClaimsResponse>>
    {
        #region Fields
        private readonly IStringLocalizer<SharedResources> stringLocalizer;
        private readonly IAuthorizationService authorizationService;
        private readonly UserManager<User> userManager;
        private readonly IMapper mapper;

        #endregion

        #region Constructor
        public ClaimQueryHandler(IStringLocalizer<SharedResources> stringLocalizer, IMapper mapper,
            IAuthorizationService authorizationService, UserManager<User> userManager) : base(stringLocalizer)
        {
            this.stringLocalizer = stringLocalizer;
            this.authorizationService = authorizationService;
            this.userManager = userManager;
            this.mapper = mapper;
        }
        #endregion

        #region Functions
        public async Task<Response<ManagerUserClaimsResponse>> Handle(ManageUserClaimsQuery request, CancellationToken cancellationToken)
        {
            var user = await userManager.FindByIdAsync(request.UserId.ToString());
            if (user == null) return NotFound<ManagerUserClaimsResponse>(stringLocalizer[SharedResourcesKeys.UserIsNotFound]);
            var result = await authorizationService.ManageUserClaimsData(user);
            return Success(result);
        }
        #endregion
    }
}
