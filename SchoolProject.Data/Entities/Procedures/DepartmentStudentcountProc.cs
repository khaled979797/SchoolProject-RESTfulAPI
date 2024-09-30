using SchoolProject.Data.Commons;

namespace SchoolProject.Data.Entities.Procedures
{
    public class DepartmentStudentcountProc : GeneralLocalizableEntity
    {
        public int DID { get; set; }
        public string? DNameAr { get; set; }
        public string? DNameEn { get; set; }
        public int StudentCount { get; set; }
    }

    public class DepartmentStudentcountProcParameters
    {
        public int DID { get; set; } = 0;
    }
}
