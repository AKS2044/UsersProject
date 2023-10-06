using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using UsersProject.Data.Models;
using UsersProject.Logic.Interfaces;
using UsersProject.Logic.Models;

namespace UsersProject.Logic.Managers
{
    /// <inheritdoc cref="IUserManager"/>
    public class RoleManager : IRoleManager
    {
        private readonly IRepositoryManager<Role> _roleRepository;
        private readonly ILogger _logger;
        public RoleManager(IRepositoryManager<Role> roleRepository, ILogger<UserManager> logger)
        {
            _roleRepository = roleRepository ?? throw new ArgumentNullException(nameof(roleRepository));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task CreateAsync(RoleDto roleDto)
        {
            try
            {
                bool isRoleExists = await _roleRepository.GetAll().AnyAsync(role => role.UserRole == roleDto.UserRole);
                if (isRoleExists)
                {
                    _logger.LogWarning("Role {RoleName} already exists in the database.", roleDto.UserRole);
                    throw new InvalidOperationException($"{roleDto.UserRole} already exists in the database.");
                }

                Role role = new Role()
                {
                    UserRole = roleDto.UserRole,
                };

                await _roleRepository.CreateAsync(role);
                await _roleRepository.SaveChangesAsync();

                _logger.LogInformation("Role {RoleName} has been successfully created.", role.UserRole);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Role {RoleName} already exists in the database.", roleDto.UserRole);
                throw new InvalidOperationException($"Role {roleDto.UserRole} already exists in the database.");
            }
        }

        public async Task DeleteAsync(int id)
        {
            try
            {
                var role = await _roleRepository.GetAll().FirstOrDefaultAsync(r => r.Id == id);

                if (role != null)
                {
                    _roleRepository.Delete(role);
                    await _roleRepository.SaveChangesAsync();
                }
                else
                {
                    _logger.LogError("Role not found in the database. Role ID: {RoleId}", id);
                    throw new InvalidOperationException($"Role not found in the database. Role ID: {id}");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while deleting the role. Role ID: {RoleId}", id);
                throw;
            }
        }

        public async Task<IEnumerable<RoleDto>> GetAllAsync()
        {
            try
            {
                var roles = await _roleRepository.GetAll().ToListAsync();

                var roleDtos = roles.Select(item => new RoleDto
                {
                    Id = item.Id,
                    UserRole = item.UserRole,
                }).ToList();

                return roleDtos;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while retrieving roles.");
                throw new ApplicationException("An error occurred while retrieving roles.", ex);
            }
        }

    }
}