namespace UsersProject.Data.Models
{
    /// <summary>
    /// User role.
    /// </summary>
    public class UserRole
    {
        /// <summary>
        /// Identification.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// User identification.
        /// </summary>
        public int UserId { get; set; }

        /// <summary>
        /// Navigation property for User.
        /// </summary>
        public User? User { get; set; }

        /// <summary>
        /// Role identification.
        /// </summary>
        public int RoleId { get; set; }

        /// <summary>
        /// Navigation property for Role.
        /// </summary>
        public Role? Role { get; set; }
    }
}