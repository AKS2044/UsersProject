using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using UsersProject.Data.Constants;
using UsersProject.Data.Models;

namespace UsersProject.Data.Configurations
{
    /// <summary>
    /// EF Configuration for UserRoles entity.
    /// </summary>
    public class UserRoleConfiguration : IEntityTypeConfiguration<UserRole>
    {
        public void Configure(EntityTypeBuilder<UserRole> builder)
        {
            builder = builder ?? throw new ArgumentNullException(nameof(builder));

            builder.ToTable(Table.UserRoles, Schema.User)
                .HasKey(userRole => userRole.Id);

            builder.Property(userRole => userRole.Id)
                .UseIdentityColumn();

            builder.HasOne(userRole => userRole.User)
               .WithMany(user => user.UserRoles)
               .HasForeignKey(userRole => userRole.UserId)
               .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(userRole => userRole.Role)
                .WithMany(role => role.UserRoles)
                .HasForeignKey(userRole => userRole.RoleId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}