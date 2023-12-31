﻿using System.ComponentModel.DataAnnotations;

namespace UsersProject.WebApi.Contracts.Requests
{
    public class UserCreateRequest
    {
        /// <summary>
        /// User Email.
        /// </summary>
        [Required(ErrorMessage = "Email is required")]
        [EmailAddress(ErrorMessage = "Invalid email format")]
        public string? Email { get; set; }

        /// <summary>
        /// User Name.
        /// </summary>
        [Required(ErrorMessage = "Name is required")]
        public string? Name { get; set; }

        /// <summary>
        /// User Age.
        /// </summary>
        [Required(ErrorMessage = "Age is required")]
        [Range(1, int.MaxValue, ErrorMessage = "Age must be greater than zero")]
        public int Age { get; set; }
    }
}