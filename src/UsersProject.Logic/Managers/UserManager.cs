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

        public UserManager(
            IRepositoryManager<User> userRepository, 
            IRepositoryManager<Role> roleRepository, 
            IRepositoryManager<UserRole> userRoleRepository, 
            ILogger<UserManager> logger)
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

        public async Task<UserDto> FindUserByIdAsync(int id)
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
                _logger.LogError(ex, "An error occurred while retrieving user by ID {id}. Error: {ErrorMessage}", id);
                throw;
            }
        }

        public async Task<IEnumerable<UserDto>> GetAllAsync(
            int pageNumber,
            int pageSize,
            string sortColumn,
            string sortDirection,
            string? filterName,
            string? filterEmail,
            string? filterAge,
            string? filterRole)
        {
            try
            {
                var query = _userRepository.GetAll();
                var userRole = await _userRoleRepository.GetAll().ToListAsync();
                

                int size = query.Count();
                
                if (!string.IsNullOrEmpty(filterName))
                {
                    query = query.Where(u => u.Name.Contains(filterName));
                }

                if (!string.IsNullOrEmpty(filterEmail))
                {
                    query = query.Where(u => u.Email.Contains(filterEmail));
                }

                if (!string.IsNullOrEmpty(filterAge))
                {
                    if (int.TryParse(filterAge, out int age))
                    {
                        query = query.Where(u => u.Age == age);
                    }
                    else
                    {
                        return Enumerable.Empty<UserDto>();
                    }
                }

                if (!string.IsNullOrEmpty(filterRole))
                {
                    query = query.Where(u => u.UserRoles.Any(ur => ur.Role.UserRole == filterRole));
                }

                if (size <= pageSize)
                {
                    pageNumber = 1;
                }

                bool isAscending = sortDirection.Equals("asc", StringComparison.OrdinalIgnoreCase);

                switch (sortColumn.ToLower())
                {
                    case "id":
                        query = isAscending ? query.OrderBy(u => u.Id) : query.OrderByDescending(u => u.Id);
                        break;
                    case "name":
                        query = isAscending ? query.OrderBy(u => u.Name) : query.OrderByDescending(u => u.Name);
                        break;
                    case "email":
                        query = isAscending ? query.OrderBy(u => u.Email) : query.OrderByDescending(u => u.Email);
                        break;
                    case "age":
                        query = isAscending ? query.OrderBy(u => u.Age) : query.OrderByDescending(u => u.Age);
                        break;
                    case "role":
                        query = isAscending
                            ? query.OrderBy(u => u.UserRoles.Select(ur => ur.Role.UserRole).FirstOrDefault())
                            : query.OrderByDescending(u => u.UserRoles.Select(ur => ur.Role.UserRole).FirstOrDefault());
                        break;
                    default:
                        query = isAscending ? query.OrderBy(u => u.Id) : query.OrderByDescending(u => u.Id);
                        break;
                }

                int skip = (pageNumber - 1) * pageSize;

                query = query.Skip(skip).Take(pageSize);

                var users = await query.Select(u => new UserDto
                {
                    Id = u.Id,
                    Name = u.Name,
                    Email = u.Email,
                    Age = u.Age
                }).ToListAsync();

                return users;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while retrieving users from the database.");
                throw new InvalidOperationException("Unable to retrieve users from the database.");
            }
        }

        public async Task DeleteAsync(int id)
        {
            try
            {
                var user = await _userRepository.GetAll().FirstOrDefaultAsync(u => u.Id == id);

                if (user == null)
                {
                    _logger.LogWarning($"User with ID {id} not found.");
                    throw new NotFoundException($"User with ID {id} not found.");
                }

                var userRoles = await _userRoleRepository.GetAll().Where(ur => ur.UserId == user.Id).ToListAsync();

                _userRoleRepository.DeleteRange(userRoles);
                await _userRoleRepository.SaveChangesAsync();

                _userRepository.Delete(user);
                await _userRepository.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while retrieving user by ID {id}.", id);
                throw;
            }
        }

        public async Task UpdateAsync(UserDto userDto)
        {
            try
            {
                var user = await _userRepository.GetAll().FirstOrDefaultAsync(u => u.Id == userDto.Id);

                if (user == null)
                {
                    _logger.LogWarning($"User with ID {userDto.Id} not found.");
                    throw new NotFoundException($"User with ID {userDto.Id} not found.");
                }

                bool isUpdated = false;

                if (userDto.Name != user.Name)
                {
                    user.Name = userDto.Name;
                    isUpdated = true;
                }

                if (userDto.Email != user.Email)
                {
                    user.Email = userDto.Email;
                    isUpdated = true;
                }

                if (userDto.Age != user.Age)
                {
                    user.Age = userDto.Age;
                    isUpdated = true;
                }

                if (isUpdated)
                {
                    _userRepository.Update(user);
                    await _userRepository.SaveChangesAsync();
                    _logger.LogInformation($"User with ID {userDto.Id} has been updated.");
                }
                else
                {
                    _logger.LogInformation($"No changes detected for user with ID {userDto.Id}.");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while updating user information {userDto}.", userDto);
                throw;
            }
        }

        public async Task SetRoleAsync(int id, string roleName)
        {
            try
            {
                var user = await _userRepository.GetAll().FirstOrDefaultAsync(u => u.Id == id);
                var role = await _roleRepository.GetAll().FirstOrDefaultAsync(r => r.UserRole == roleName);

                if (user == null || role == null)
                {
                    _logger.LogWarning($"User or role not found.");
                    throw new NotFoundException($"User or role not found.");
                }

                var userRole = new UserRole()
                {
                    UserId = user.Id,
                    RoleId = role.Id
                };

                _logger.LogInformation("New role successfully added for user.", user);
                await _userRoleRepository.CreateAsync(userRole);
                await _userRoleRepository.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "User{id} or role{roleName} not found. Error: {ErrorMessage}", id);
                throw;
            }
        }
    }
}