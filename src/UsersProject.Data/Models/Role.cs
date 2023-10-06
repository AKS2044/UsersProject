namespace UsersProject.Data.Models
{
    /// <summary>
    /// User role model.
    /// </summary>
    public class Role
    {
        /// <summary>
        /// Identification.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Navigation property for UserRole.
        /// </summary>
        public ICollection<UserRole>? UserRoles { get; set; }

        /// <summary>
        /// Role.
        /// </summary>
        public string?  UserRole { get; set; }
    }
}