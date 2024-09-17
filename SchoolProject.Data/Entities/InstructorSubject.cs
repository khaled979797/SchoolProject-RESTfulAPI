using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SchoolProject.Data.Entities
{
    public class InstructorSubject
    {
        [Key]
        public int InsId { get; set; }

        [Key]
        public int SubId { get; set; }

        [ForeignKey(nameof(InsId))]
        [InverseProperty(nameof(Instructor.InstructorSubjects))]
        public virtual Instructor? Instructor { get; set; }


        [ForeignKey(nameof(SubId))]
        [InverseProperty(nameof(Subjects.InstructorSubjects))]
        public virtual Subjects? Subject { get; set; }
    }
}
