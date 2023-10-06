using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Serilog;
using UsersProject.Data.Models;
using UsersProject.Logic.Interfaces;
using UsersProject.Logic.Models;
using UsersProject.WebApi.Contracts.Requests;
using UsersProject.WebApi.Contracts.Responses;
using UsersProject.WebApi.Settings;

namespace UsersProject.WebApi.Controllers
{
    [Route("api/[controller]")]
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

        [HttpPost("create")]
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
                return BadRequest($"An error occurred while creating a user. {ex}");
            }
        }

        [HttpGet("getAll")]
        public async Task<IActionResult> GetAllUsersAsync()
        {
            try
            {
                var users = await _userManager.GetAllAsync();

                return Ok(users);
            }
            catch (Exception ex)
            {
                return BadRequest(ex);
            }
        }
    }
}
