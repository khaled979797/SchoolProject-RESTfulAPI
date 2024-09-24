namespace SchoolProject.Data.AppMetaData
{
    public static class Router
    {
        public const string SingleRoute = "/{id}";
        public const string root = "Api";
        public const string version = "V1";
        public const string Rule = root + "/" + version + "/";

        public static class StudentRouting
        {
            public const string Prefix = Rule + "Student";
            public const string List = Prefix + "/List";
            public const string GetById = Prefix + SingleRoute;
            public const string Create = Prefix + "/Create";
            public const string Edit = Prefix + "/Edit";
            public const string Delete = Prefix + SingleRoute;
            public const string Paginated = Prefix + "/Paginated";
        }

        public static class DepartmentRouting
        {
            public const string Prefix = Rule + "Department";
            public const string List = Prefix + "/List";
            public const string GetById = Prefix + "/Id";
            public const string Create = Prefix + "/Create";
            public const string Edit = Prefix + "/Edit";
            public const string Delete = Prefix + SingleRoute;
            public const string Paginated = Prefix + "/Paginated";
        }

        public static class UserRouting
        {
            public const string Prefix = Rule + "User";
            public const string GetById = Prefix + SingleRoute;
            public const string Create = Prefix + "/Create";
            public const string Edit = Prefix + "/Edit";
            public const string Delete = Prefix + SingleRoute;
            public const string Paginated = Prefix + "/Paginated";
            public const string ChangePassword = Prefix + "/ChangePassword";
        }

        public static class AuthenticationRouting
        {
            public const string Prefix = Rule + "Authentication";
            public const string SignIn = Prefix + "/SignIn";
            public const string RefreshToken = Prefix + "/RefreshToken";
            public const string ValidateToken = Prefix + "/ValidateToken";
        }

        public static class AuthorizationRouting
        {
            public const string Prefix = Rule + "Authorization";
            public const string Role = Prefix + "/Role";
            public const string Claim = Prefix + "/Claim";
            public const string CreateRole = Role + "/Create";
            public const string EditRole = Role + "/Edit";
            public const string DeleteRole = Role + SingleRoute;
            public const string RoleList = Role + "/List";
            public const string GetRoleById = Role + SingleRoute;
            public const string ManageUserRoles = Role + "/ManageUserRoles" + "/{userId}";
            public const string EditUserRoles = Role + "/EditUserRoles";
            public const string ManageUserClaims = Claim + "/ManageUserClaims" + "/{userId}";
            public const string EditUserClaims = Claim + "/EditUserClaims";
        }
    }
}
