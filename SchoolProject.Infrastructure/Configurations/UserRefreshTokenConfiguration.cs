using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SchoolProject.Data.Entities.Identity;

namespace SchoolProject.Infrastructure.Configurations
{
    class UserRefreshTokenConfiguration : IEntityTypeConfiguration<UserRefreshToken>
    {
        public void Configure(EntityTypeBuilder<UserRefreshToken> builder)
        {
            builder.HasKey(r => r.Id);
            builder.HasOne(r => r.User)
                .WithMany(u => u.UserRefreshTokens)
                .HasForeignKey(r => r.UserId);
        }
    }
}
