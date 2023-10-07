using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Serilog;
using System.Net.Mime;
using UsersProject.Logic.Interfaces;
using UsersProject.Logic.Models;
using UsersProject.WebApi.Contracts.Requests;
using UsersProject.WebApi.Contracts.Responses;
using UsersProject.WebApi.Settings;

namespace UsersProject.WebApi.Controllers
{
    [Route("api/[controller]")]
    [Produces(MediaTypeNames.Application.Json)]
    [Consumes(MediaTypeNames.Application.Json)]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IUserManager _userManager;
        public readonly IWebHostEnvironment _appEnvironment;
        private readonly IJwtService _jwtService;
        private readonly AppSettings _appSettings;

        public UserController(
            IUserManager userManager,
            IJwtService jwtService,
            IOptions<AppSettings> appSettings,
            IWebHostEnvironment appEnvironment)
        {
            _userManager = userManager ?? throw new ArgumentNullException(nameof(userManager));
            _jwtService = jwtService ?? throw new ArgumentNullException(nameof(jwtService));
            _appEnvironment = appEnvironment ?? throw new ArgumentNullException(nameof(appEnvironment));
            _appSettings = appSettings.Value ?? throw new ArgumentNullException(nameof(appSettings));
        }

        /// <summary>
        /// Create user
        /// </summary>
        /// <param name="request">User request model</param>
        /// <returns>User data</returns>
        [HttpPost("create")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> CreateAsync(UserCreateRequest request)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    Log.Warning("Invalid model data.");
                    return BadRequest("Invalid model data.");
                }

                var userDto = new UserDto
                {
                    Name = request.Name,
                    Email = request.Email,
                    Age = request.Age
                };

                var user = await _userManager.CreateAsync(userDto);

                var token = _jwtService.GenerateJwtToken(user.Id, _appSettings.Secret);
                var userRoles = await _userManager.GetUserRolesByIdAsync(user.Id);
                var response = new AuthenticateResponse(user, token, userRoles);

                Log.Information("User {Email} has been successfully created.", user.Email);

                return Ok(response);
            }
            catch (Exception ex)
            {
                Log.Error(ex, "An error occurred while creating a user.");
                return StatusCode(404, $"An error occurred while creating a user. {ex}");
            }
        }

        /// <summary>
        /// Delete user by ID
        /// </summary>
        /// <param name="id">User Id</param>
        [HttpDelete("delete")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> DeleteAsync(int id)
        {
            try
            {
                if (id <= 0)
                {
                    Log.Information("{Id} must be greater than zero", id);
                    return BadRequest(new { message = "Id must be greater than zero" });
                }

                await _userManager.DeleteAsync(id);

                Log.Information("User {id} has been successfully deleted.", id);

                return Ok();
            }
            catch (Exception ex)
            {
                Log.Error(ex, "An error occurred while deleting a user.");
                return StatusCode(404, $"An error occurred while deleting a user. {ex.Message}");
            }
        }

        /// <summary>
        /// Get user by ID
        /// </summary>
        /// <param name="id">User Id</param>
        /// <returns>User data</returns>
        [HttpGet("getUser")]
        [ProducesResponseType(typeof(UserResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetUserByIdAsync(int id)
        {
            try
            {
                var user = await _userManager.FindUserByIdAsync(id);
                var roles = await _userManager.GetUserRolesByIdAsync(id);

                if (user == null)
                {
                    Log.Information("User {id} was not received", id);
                    return BadRequest(new { message = "User {id} is not found." });
                }

                var response = new UserResponse()
                {
                    Id = user.Id,
                    Name = user.Name,
                    Email = user.Email,
                    Age = user.Age,
                    Roles = roles
                };

                Log.Information("User {id} has been successfully getting.", id);

                return Ok(response);
            }
            catch (Exception ex)
            {
                Log.Information($"User {id} was not received. Error: {ex}", id);
                return StatusCode(404, $"User {id} is not found.");
            }
        }

        /// <summary>
        /// Add role for user
        /// </summary>
        /// <param name="id">User Id</param>
        /// <param name="roleName">Role name. You can add next names: User, Admin, Support, SuperAdmin</param>
        [HttpPost("addRole")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> AddRoleForUserAsync(int id, string roleName)
        {
            try
            {
                await _userManager.SetRoleAsync(id, roleName);

                Log.Information("Role {roleName} has been successfully setting.", roleName);

                return Ok();
            }
            catch (Exception ex)
            {
                Log.Information($"User or role is not found. Error: {ex}", id);
                return StatusCode(404, $"User or role is not found.");
            }
        }

        /// <summary>
        /// Get all users with pagination, sorting, and filtering options
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
        [HttpGet("getAll")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> GetAllUsersAsync(
            int pageNumber = 1,
            int pageSize = 10,
            string sortColumn = "Id",
            string sortDirection = "asc",
            string? filterName = null,
            string? filterEmail = null,
            string? filterAge = null,
            string? filterRole = null)
        {
            try
            {
                var users = await _userManager.GetAllAsync(
                    pageNumber,
                    pageSize,
                    sortColumn,
                    sortDirection,
                    filterName,
                    filterEmail,
                    filterAge,
                    filterRole);

                Log.Information("All users has been successfully getting.");

                return Ok(users);
            }
            catch (Exception ex)
            {
                Log.Error(ex, $"An error occurred while fetching users data. Error: {ex}");
                return StatusCode(500, $"An error occurred while fetching users data. Error: {ex}");
            }
        }

        /// <summary>
        /// User Update
        /// </summary>
        /// <param name="id">User Id</param>
        /// <param name="request">Кequest user model</param>
        /// <returns>Updated user data</returns>
        [HttpPut("update")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> UpdateAsync(int id, [FromBody] UserUpdateRequest request)
        {
            try
            {
                var user = await _userManager.FindUserByIdAsync(id);

                if (user == null)
                {
                    Log.Information($"User with ID {id} was not found.");
                    return BadRequest($"User with ID {id} was not found.");
                }
                user.Id = id;
                user.Name = request.Name;
                user.Email = request.Email;
                user.Age = request.Age;

                await _userManager.UpdateAsync(user);

                Log.Information($"User with ID {id} has been updated.");

                return Ok(user);
            }
            catch (Exception ex)
            {
                Log.Error(ex, $"An error occurred while updating user with ID {id}. Error: {ex}");
                return StatusCode(400, $"An error occurred while updating user data. Error: {ex}");
            }
        }
    }
}