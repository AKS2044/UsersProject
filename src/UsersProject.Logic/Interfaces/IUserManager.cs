using UsersProject.Data.Models;
using UsersProject.Logic.Models;

namespace UsersProject.Logic.Interfaces
{
    /// <summary>
    /// User manager.
    /// </summary>
    public interface IUserManager
    {
        /// <summary>
        /// Create new user.
        /// </summary>
        /// <param name="userDto">User data transfer object.</param>
        Task<User> CreateAsync(UserDto userDto);

        /// <summary>
        /// Get user roles.
        /// </summary>
        /// <param name="id">User identifier.</param>
        Task<IEnumerable<RoleDto>> GetUserRolesByIdAsync(int id);

        /// <summary>
        /// Get user by id.
        /// </summary>
        /// <param name="id">User identifier.</param>
        Task<UserDto> FindByIdAsync(int id);

        /// <summary>
        /// Get all users.
        /// </summary>
        Task<IEnumerable<UserDto>> GetAllAsync();
    }
}