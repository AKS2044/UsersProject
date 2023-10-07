using UsersProject.Logic.Models;

namespace UsersProject.WebApi.Contracts.Responses
{
    /// <summary>
    /// User response model
    /// </summary>
    public class UserResponse
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
        /// Roles.
        /// </summary>
        public IEnumerable<RoleDto>? Roles { get; set; }
    }
}
