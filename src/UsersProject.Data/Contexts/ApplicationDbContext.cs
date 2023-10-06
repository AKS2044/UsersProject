using Microsoft.EntityFrameworkCore;
using UsersProject.Data.Configurations;
using UsersProject.Data.Models;

namespace UsersProject.Data.Contexts
{
    /// <summary>
    /// Main application context
    /// </summary>
    public class ApplicationDbContext: DbContext
    {
        /// <summary>
        /// DbSet for Users.
        /// </summary>
        public DbSet<User> Users { get; set; }

        /// <summary>
        /// DbSet for Roles.
        /// </summary>
        public DbSet<Role> Roles { get; set; }

        /// <summary>
        /// DbSet for UserRoles.
        /// </summary>
        public DbSet<UserRole> UserRoles { get; set; }

        /// <summary>
        /// Contructor with params.
        /// </summary>
        /// <param name="options">Database context options.</param>
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder = builder ?? throw new ArgumentNullException(nameof(builder));

            builder.ApplyConfiguration(new UserConfiguration());
            builder.ApplyConfiguration(new RoleConfiguration());
            builder.ApplyConfiguration(new UserRoleConfiguration());

            base.OnModelCreating(builder);
        }

        //protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        //{
        //    optionsBuilder.UseSqlServer(@"Server=(localdb)\mssqllocaldb;Database=UserProject;Trusted_Connection=True;");
        //}
    }
}