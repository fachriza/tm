using System;
using System.ComponentModel.DataAnnotations;

namespace AspNetUserApi.DTOs
{
    /// <summary>
    /// Data Transfer Object for creating a new user with input validation.
    /// </summary>
    public class CreateUserDto
    {
        [Required(ErrorMessage = "FirstName is required.")]
        [StringLength(50, MinimumLength = 1, ErrorMessage = "FirstName must be between 1 and 50 characters.")]
        public string FirstName { get; set; } = string.Empty;

        [Required(ErrorMessage = "LastName is required.")]
        [StringLength(50, MinimumLength = 1, ErrorMessage = "LastName must be between 1 and 50 characters.")]
        public string LastName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Email is required.")]
        [EmailAddress(ErrorMessage = "Invalid email address format.")]
        [StringLength(100, ErrorMessage = "Email must not exceed 100 characters.")]
        public string Email { get; set; } = string.Empty;
    }

    /// <summary>
    /// Data Transfer Object for updating an existing user.
    /// </summary>
    public class UpdateUserDto
    {
        [Required(ErrorMessage = "FirstName is required.")]
        [StringLength(50, MinimumLength = 1, ErrorMessage = "FirstName must be between 1 and 50 characters.")]
        public string FirstName { get; set; } = string.Empty;

        [Required(ErrorMessage = "LastName is required.")]
        [StringLength(50, MinimumLength = 1, ErrorMessage = "LastName must be between 1 and 50 characters.")]
        public string LastName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Email is required.")]
        [EmailAddress(ErrorMessage = "Invalid email address format.")]
        [StringLength(100, ErrorMessage = "Email must not exceed 100 characters.")]
        public string Email { get; set; } = string.Empty;
    }

    /// <summary>
    /// Standardized User response model returned to clients.
    /// </summary>
    public class UserResponseDto
    {
        public int UserId { get; set; }
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }

    /// <summary>
    /// Standardized JSON error response object.
    /// </summary>
    public class ApiErrorResponse
    {
        public int StatusCode { get; set; }
        public string Message { get; set; } = string.Empty;
        public string[]? Details { get; set; }
        public DateTime Timestamp { get; set; } = DateTime.UtcNow;
    }
}
