namespace UsersProject.Logic.Models
{
    /// <summary>
    /// User role.
    /// </summary>
    public class UserRoleDto
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
        /// Role identification.
        /// </summary>
        public int RoleId { get; set; }
    }
}