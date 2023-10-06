using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using UsersProject.Data.Constants;
using UsersProject.Data.Models;

namespace UsersProject.Data.Configurations
{
    /// <summary>
    /// EF Configuration for Role entity.
    /// </summary>
    public class RoleConfiguration : IEntityTypeConfiguration<Role>
    {
        public void Configure(EntityTypeBuilder<Role> builder)
        {
            builder = builder ?? throw new ArgumentNullException(nameof(builder));

            builder.ToTable(Table.Roles, Schema.User)
                .HasKey(role => role.Id);

            builder.Property(role => role.Id)
                .UseIdentityColumn();
        }
    }
}