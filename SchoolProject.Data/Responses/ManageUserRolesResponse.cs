namespace SchoolProject.Data.Responses
{
    public class ManageUserRolesResponse
    {
        public int UserId { get; set; }
        public List<UserRole> UserRoles { get; set; }
    }

    public class UserRole
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public bool HasRole { get; set; }
    }
}