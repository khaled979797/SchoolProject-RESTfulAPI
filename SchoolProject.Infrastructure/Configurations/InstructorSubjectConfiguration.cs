using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SchoolProject.Data.Entities;

namespace SchoolProject.Infrastructure.Configurations
{
    public class InstructorSubjectConfiguration : IEntityTypeConfiguration<InstructorSubject>
    {
        public void Configure(EntityTypeBuilder<InstructorSubject> builder)
        {
            builder.HasKey(x => new { x.InsId, x.SubId });

            builder.HasOne(x => x.Instructor)
            .WithMany(i => i.InstructorSubjects)
            .HasForeignKey(x => x.InsId);

            builder.HasOne(x => x.Subject)
            .WithMany(i => i.InstructorSubjects)
            .HasForeignKey(x => x.SubId);
        }
    }
}
