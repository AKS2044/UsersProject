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
        /// Delete user.
        /// </summary>
        /// <param name="id">User identifie.</param>
        Task DeleteAsync(int id);

        /// <summary>
        /// User update.
        /// </summary>
        /// <param name="userDto">User data transfer object.</param
        Task UpdateAsync(UserDto userDto);

        /// <summary>
        /// Get user roles.
        /// </summary>
        /// <param name="id">User identifier.</param>
        Task<IEnumerable<RoleDto>> GetUserRolesByIdAsync(int id);

        /// <summary>
        /// Get user by id.
        /// </summary>
        /// <param name="id">User identifier.</param>
        Task<UserDto> FindUserByIdAsync(int id);

        /// <summary>
        /// Set role for user.
        /// </summary>
        /// <param name="id">User identifier.</param>
        /// <param name="roleName">Role name.</param>
        Task SetRoleAsync(int id, string roleName);

        /// <summary>
        /// Get all users.
        /// </summary>
        /// <param name="pageNumber">Page number (starting from 1)</param>
        /// <param name="pageSize">Number of users per page</param>
        /// <param name="sortColumn">Name of the column to sort by</param>
        /// <param name="sortDirection">Sorting direction (asc or desc)</param>
        /// <param name="filterName">Filter name to apply to user data</param>
        /// <param name="filterEmail">Filter email to apply to user data</param>
        /// <param name="filterAge">Filter age to apply to user data</param>
        /// <param name="filterRole">Filter role to apply to user data</param>
        /// <returns>Paginated and filtered user data</returns>
        Task<IEnumerable<UserDto>> GetAllAsync(
            int pageNumber,
            int pageSize,
            string sortColumn,
            string sortDirection,
            string? filterName,
            string? filterEmail,
            string? filterAge,
            string? filterRole);
    }
}