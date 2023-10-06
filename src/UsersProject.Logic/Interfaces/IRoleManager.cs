using UsersProject.Logic.Models;

namespace UsersProject.Logic.Interfaces
{
    /// <summary>
    /// Role manager.
    /// </summary>
    public interface IRoleManager
    {
        /// <summary>
        /// Create new Role.
        /// </summary>
        /// <param name="roleDto">Role data transfer object.</param>
        Task CreateAsync(RoleDto roleDto);

        /// <summary>
        /// Get all roles.
        /// </summary>
        Task<IEnumerable<RoleDto>> GetAllAsync();

        /// <summary>
        /// Delete role by identifier.
        /// </summary>
        /// <param name="id">Role identifier.</param>
        Task DeleteAsync(int id);
    }
}