using UsersProject.WebApi.Models;
using UsersProject.WebApi.Settings;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Text;
using UsersProject.Logic.Interfaces;
using Serilog;
using Microsoft.AspNetCore.Identity;
using UsersProject.Data.Models;

namespace UsersProject.WebApi.Middlewares
{
    /// <summary>
    /// Jwt middleware.
    /// </summary>
    public class JwtMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly AppSettings _appSettings;
        private readonly IServiceScopeFactory _serviceScopeFactory;

        /// <summary>
        /// Constructor with params.
        /// </summary>
        /// <param name="next">Next request delegate.</param>
        /// <param name="appSettings">App settings.</param>
        /// <param name="serviceScopeFactory">Service scope factory.</param>
        public JwtMiddleware(
            RequestDelegate next,
            IOptions<AppSettings> appSettings,
            IServiceScopeFactory serviceScopeFactory)
        {
            _next = next ?? throw new ArgumentNullException(nameof(next));
            _serviceScopeFactory = serviceScopeFactory ?? throw new ArgumentNullException(nameof(serviceScopeFactory));
            _appSettings = appSettings.Value ?? throw new ArgumentNullException(nameof(appSettings));
        }

        public async Task Invoke(HttpContext context)
        {
            var token = context.Request.Headers["Authorization"].FirstOrDefault()?.Split(" ").Last();

            if (token != null)
            {
                await AttachUserToContextAsync(context, token);
            }

            await _next(context);
        }

        private async Task AttachUserToContextAsync(HttpContext context, string token)
        {
            try
            {
                if (_appSettings.Secret == null) return;

                var tokenHandler = new JwtSecurityTokenHandler();
                var key = Encoding.ASCII.GetBytes(_appSettings.Secret);
                tokenHandler.ValidateToken(token, new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ValidateIssuer = false,
                    ValidateAudience = false,
                    // set clockskew to zero so tokens expire exactly at token expiration time (instead of 5 minutes later)
                    ClockSkew = TimeSpan.Zero
                }, out SecurityToken validatedToken);

                var jwtToken = (JwtSecurityToken)validatedToken;
                var userId = jwtToken.Claims.First(x => x.Type == "id").Value;

                // attach user to context on successful jwt validation
                using var scope = _serviceScopeFactory.CreateScope();
                var userManager = scope.ServiceProvider.GetService<IUserManager>();

                if (int.TryParse(userId, out int id))
                {
                    if (userManager != null)
                    {
                        var user = await userManager.FindByIdAsync(id);
                        var role = await userManager.GetUserRolesByIdAsync(id);
                        var userModel = new UserModel
                        {
                            Id = user.Id,
                            Name = user.Name,
                            Email = user.Email,
                            Roles = role
                        };

                        context.Items["User"] = userModel;
                    }
                }
                else
                {
                    Log.Error("You can't parse userId to integer");
                }
            }
            catch(Exception ex)
            {
                Log.Error(ex, "An error occurred while processing JWT token.");
            }
        }
    }
}