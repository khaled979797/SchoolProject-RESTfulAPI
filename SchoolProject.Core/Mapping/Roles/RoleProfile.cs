﻿using AutoMapper;

namespace SchoolProject.Core.Mapping.Roles
{
    public partial class RoleProfile : Profile
    {
        public RoleProfile()
        {
            GetRolesListQueryMapping();
            GetRoleByIdQueryMapping();
        }
    }
}
