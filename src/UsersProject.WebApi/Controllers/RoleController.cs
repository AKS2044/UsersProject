using Microsoft.AspNetCore.Mvc;
using Serilog;
using System.Net.Mime;
using UsersProject.Logic.Interfaces;
using UsersProject.Logic.Models;

namespace UsersProject.WebApi.Controllers
{
    [Route("api/[controller]")]
    [Produces(MediaTypeNames.Application.Json)]
    [Consumes(MediaTypeNames.Application.Json)]
    [ApiController]
    public class RoleController : ControllerBase
    {
        private readonly IRoleManager _roleManager;
        public readonly IWebHostEnvironment _appEnvironment;

        public RoleController(
            IRoleManager roleManager,
            IWebHostEnvironment appEnvironment)
        {
            _roleManager = roleManager ?? throw new ArgumentNullException(nameof(roleManager));
            _appEnvironment = appEnvironment ?? throw new ArgumentNullException(nameof(appEnvironment));
        }

        /// <summary>
        /// Create role
        /// </summary>
        /// <param name="roleName">Role name</param>
        [HttpPost("create")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> CreateAsync(string roleName)
        {
            try
            {
                var role = new RoleDto
                {
                    UserRole = roleName
                };

                await _roleManager.CreateAsync(role);

                Log.Information("Role {RoleName} has been successfully created.", roleName);

                return Ok();
            }
            catch (Exception error)
            {
                Log.Error(error, $"{roleName} already exists in the database.");
                return StatusCode(400, $"{roleName} already exists in the database.");
            }
        }

        /// <summary>
        /// Return all roles
        /// </summary>
        /// <returns>Roles data</returns>
        [HttpGet("getAll")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> GetAllRolesAsync()
        {
            try
            {
                var roles = await _roleManager.GetAllAsync();

                Log.Information("Role has been successfully getting.");

                return Ok(roles);
            }
            catch (Exception ex)
            {
                Log.Error(ex, $"Couldn't get roles. Error: {ex}");
                return StatusCode(400, $"Couldn't get roles. Error: {ex}");
            }
        }

        /// <summary>
        /// Delete role by id
        /// </summary>
        /// <param name="id">Role Id</param>
        [HttpDelete("delete")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> DeleteRoleAsync(int id)
        {
            try
            {
                await _roleManager.DeleteAsync(id);

                Log.Information("Role {id} has been successfully deleting.", id);

                return Ok();
            }
            catch (Exception ex)
            {
                Log.Error(ex, "An error occurred while deleting a role.");
                return StatusCode(400, $"An error occurred while deleting a role. Error: {ex}");
            }
        }

    }
}
