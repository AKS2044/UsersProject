using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Serilog;
using UsersProject.Logic.Interfaces;
using UsersProject.Logic.Models;

namespace UsersProject.WebApi.Controllers
{
    [Route("api/[controller]")]
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

        [HttpPost("create")]
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
                return BadRequest($"{roleName} already exists in the database.");
            }
        }

        [HttpGet("getAll")]
        public async Task<IActionResult> GetAllRolesAsync()
        {
            try
            {
                var roles = await _roleManager.GetAllAsync();

                return Ok(roles);
            }
            catch (Exception error)
            {
                return BadRequest(error);
            }
        }

        [HttpDelete("delete")]
        public async Task<IActionResult> DeleteRoleAsync(int id)
        {
            try
            {
                await _roleManager.DeleteAsync(id);

                return Ok();
            }
            catch (Exception error)
            {
                Log.Error(error, "An error occurred while deleting a role.");
                return BadRequest("An error occurred while deleting a role.");
            }
        }

    }
}
