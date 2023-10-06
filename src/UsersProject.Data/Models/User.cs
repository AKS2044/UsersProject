namespace UsersProject.Data.Models
{
    /// <summary>
    /// User model.
    /// </summary>
    public class User 
    {
        /// <summary>
        /// User Identification.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// User name.
        /// </summary>
        public string? Name { get; set; }

        /// <summary>
        /// User age.
        /// </summary>
        public int Age { get; set; }

        /// <summary>
        /// User Email.
        /// </summary>
        public string? Email { get; set; }

        /// <summary>
        /// Navigation property for UserRole.
        /// </summary>
        public ICollection<UserRole>? UserRoles { get; set; }
    }
}