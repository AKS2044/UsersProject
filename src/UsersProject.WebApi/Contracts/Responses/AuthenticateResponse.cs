using UsersProject.Data.Models;
using UsersProject.Logic.Models;
using UsersProject.WebApi.Models;

namespace UsersProject.WebApi.Contracts.Responses
{
    /// <summary>
    /// User authenticate response.
    /// </summary>
    public class AuthenticateResponse : UserModel
    {
        /// <summary>
        /// Constructor with params.
        /// </summary>
        /// <param name="user">User database model.</param>
        /// <param name="token">Jwt token.</param>
        public AuthenticateResponse(User user, string token, IEnumerable<RoleDto> roles)
        {
            Id = user.Id;
            Name = user.Name;
            Age = user.Age;
            Email = user.Email;
            Token = token;
            Roles = roles;
        }
    }
}