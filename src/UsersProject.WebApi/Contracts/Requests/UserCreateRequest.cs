using System.ComponentModel.DataAnnotations;
using UsersProject.Logic.Models;

namespace UsersProject.WebApi.Contracts.Requests
{
    public class UserCreateRequest
    {
        /// <summary>
        /// User Email.
        /// </summary>
        [Required]
        public string? Email { get; set; }

        /// <summary>
        /// User Name.
        /// </summary>
        [Required]
        public string? Name { get; set; }

        /// <summary>
        /// User Age.
        /// </summary>
        [Required]
        public int Age { get; set; }
    }
}