using UsersProject.Logic.Models;

namespace UsersProject.WebApi.Models
{
    /// <summary>
    /// User model.
    /// </summary>
    public class UserModel
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
        /// User token.
        /// </summary>
        public string? Token { get; set; }

        /// <summary>
        /// Roles.
        /// </summary>
        public IEnumerable<RoleDto>? Roles { get; set; }
    }
}