using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using UsersProject.Data.Models;
using UsersProject.Logic.Exceptions;
using UsersProject.Logic.Interfaces;
using UsersProject.Logic.Models;

namespace UsersProject.Logic.Managers
{
    /// <inheritdoc cref="IUserManager"/>
    public class UserManager : IUserManager
    {
        private readonly IRepositoryManager<User> _userRepository;
        private readonly IRepositoryManager<Role> _roleRepository;
        private readonly IRepositoryManager<UserRole> _userRoleRepository;
        private readonly ILogger _logger;
        public UserManager(IRepositoryManager<User> userRepository, IRepositoryManager<Role> roleRepository, IRepositoryManager<UserRole> userRoleRepository,ILogger<UserManager> logger)
        {
            _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
            _roleRepository = roleRepository ?? throw new ArgumentNullException(nameof(roleRepository));
            _userRoleRepository = userRoleRepository ?? throw new ArgumentNullException(nameof(userRoleRepository));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<User> CreateAsync(UserDto userDto)
        {
            try
            {
                var isEmailExists = await _userRepository.GetAll().FirstOrDefaultAsync(u => u.Email == userDto.Email);
                if (isEmailExists != null)
                {
                    _logger.LogWarning("Email {Email} already exists in the database.", userDto.Email);
                    throw new InvalidOperationException($"{userDto.Email} already exists in the database.");
                }

                User newUser = new User()
                {
                    Name = userDto.Name,
                    Email = userDto.Email,
                    Age = userDto.Age
                };

                await _userRepository.CreateAsync(newUser);
                await _userRepository.SaveChangesAsync();

                var role = await _roleRepository.GetAll().FirstOrDefaultAsync(r => r.UserRole == "User");

                if (role == null)
                {
                    _logger.LogError("Role 'User' not found in the database.");
                    throw new NotFoundException("Role 'User' not found in the database.");
                }

                UserRole userUserRole = new UserRole()
                {
                    UserId = newUser.Id,
                    RoleId = role.Id
                };

                await _userRoleRepository.CreateAsync(userUserRole);
                await _userRoleRepository.SaveChangesAsync();

                _logger.LogInformation("User {Email} has been successfully created.", newUser.Email);
                return newUser;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while executing CreateAsync for user {Email}.", userDto.Email);
                throw new InvalidOperationException($"An error occurred while executing CreateAsync for Email {userDto.Email}.");
            }
        }

        public async Task<IEnumerable<RoleDto>> GetUserRolesByIdAsync(int id)
        {
            try
            {
                var userExists = await _userRepository.GetAll().AnyAsync(u => u.Id == id);
                if (!userExists)
                {
                    _logger.LogWarning("User with ID {UserId} not found.", id);
                    throw new NotFoundException($"User with ID {id} not found.");
                }

                var userRoles = await _userRoleRepository.GetAll().Where(u => u.UserId == id).Select(r => r.RoleId).ToListAsync();

                if (userRoles.Count == 0)
                {
                    _logger.LogInformation("No roles found for user with ID {UserId}.", id);
                    return Enumerable.Empty<RoleDto>();
                }

                var roles = await _roleRepository.GetAll().Where(r => userRoles.Contains(r.Id)).Select(r => new RoleDto
                {
                    Id = r.Id,
                    UserRole = r.UserRole,
                }).ToListAsync();

                return roles;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while retrieving roles for user with ID {UserId}.", id);
                throw;
            }
        }
        public async Task<UserDto> FindByIdAsync(int id)
        {
            try
            {
                var user = await _userRepository.GetAll().FirstOrDefaultAsync(u => u.Id == id);

                if (user == null)
                {
                    _logger.LogWarning($"User with ID {id} not found.");
                    throw new NotFoundException($"User with ID {id} not found.");
                }

                var userDto = new UserDto
                {
                    Id = user.Id,
                    Name = user.Name,
                    Email = user.Email,
                    Age = user.Age,
                };

                return userDto;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while retrieving user by ID {UserId}. Error: {ErrorMessage}", id, ex.Message);
                throw;
            }
        }

        public async Task<IEnumerable<UserDto>> GetAllAsync()
        {
            try
            {
                var users = await _userRepository.GetAll().Select(u => new UserDto
                {
                    Id = u.Id,
                    Name = u.Name,
                    Email = u.Email,
                    Age = u.Age,
                }).ToListAsync();

                return users;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while retrieving users from the database.");
                throw new InvalidOperationException("Unable to retrieve users from the database.");
            }
        }
    }
}