using System.ComponentModel.DataAnnotations;

namespace UsersProject.WebApi.Contracts.Requests
{
    public class UserUpdateRequest
    {
        /// <summary>
        /// User Email.
        /// </summary>
        [EmailAddress(ErrorMessage = "Invalid email format")]
        public string? Email { get; set; }

        /// <summary>
        /// User Name.
        /// </summary>
        public string? Name { get; set; }

        /// <summary>
        /// User Age.
        /// </summary>
        [Range(1, int.MaxValue, ErrorMessage = "Age must be greater than zero")]
        public int Age { get; set; }
    }
}