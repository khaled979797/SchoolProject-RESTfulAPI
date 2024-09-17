using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SchoolProject.Data.Entities
{
    public class DepartmetSubject
    {
        [Key]
        public int DID { get; set; }

        [Key]
        public int SubID { get; set; }

        [ForeignKey(nameof(DID))]
        [InverseProperty(nameof(Department.DepartmentSubjects))]
        public virtual Department? Department { get; set; }

        [ForeignKey(nameof(SubID))]
        [InverseProperty(nameof(Subjects.DepartmetsSubjects))]
        public virtual Subjects? Subject { get; set; }
    }
}
