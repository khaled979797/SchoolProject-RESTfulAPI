using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SchoolProject.Data.Entities
{
    public class StudentSubject
    {
        [Key]
        public int StudID { get; set; }

        [Key]
        public int SubID { get; set; }
        public decimal? Grade { get; set; }

        [ForeignKey(nameof(StudID))]
        [InverseProperty(nameof(Student.StudentSubjects))]
        public virtual Student? Student { get; set; }

        [ForeignKey(nameof(SubID))]
        [InverseProperty(nameof(Subjects.StudentsSubjects))]
        public virtual Subjects? Subject { get; set; }
    }
}
