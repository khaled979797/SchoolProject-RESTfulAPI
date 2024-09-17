using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SchoolProject.Data.Entities;

namespace SchoolProject.Infrastructure.Configurations
{
    public class SubjectConfiguration : IEntityTypeConfiguration<Subjects>
    {
        public void Configure(EntityTypeBuilder<Subjects> builder)
        {
            builder.HasKey(s => s.SubID);

            builder.HasMany(s => s.StudentsSubjects)
                .WithOne(ss => ss.Subject)
                .HasForeignKey(ss => ss.SubID);

            builder.HasMany(s => s.DepartmetsSubjects)
                .WithOne(ds => ds.Subject)
                .HasForeignKey(ds => ds.SubID);

            builder.HasMany(s => s.InstructorSubjects)
                .WithOne(x => x.Subject)
                .HasForeignKey(x => x.SubId);
        }
    }
}
