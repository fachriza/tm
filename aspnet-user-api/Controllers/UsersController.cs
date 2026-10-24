using System.Collections.Generic;
using System.Threading.Tasks;
using AspNetUserApi.DTOs;
using AspNetUserApi.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace AspNetUserApi.Controllers
{
    /// <summary>
    /// RESTful API Controller for User Management CRUD operations.
    /// Route: /api/users
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    [Produces("application/json")]
    public class UsersController : ControllerBase
    {
        private readonly IUserService _userService;

        public UsersController(IUserService userService)
        {
            _userService = userService;
        }

        /// <summary>
        /// Retrieves a list of all users.
        /// GET: /api/users
        /// </summary>
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<UserResponseDto>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetAllUsers()
        {
            var users = await _userService.GetAllUsersAsync();
            return Ok(users);
        }

        /// <summary>
        /// Retrieves a single user by UserId.
        /// GET: /api/users/{userId}
        /// </summary>
        [HttpGet("{userId:int}")]
        [ProducesResponseType(typeof(UserResponseDto), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetUserById(int userId)
        {
            if (userId <= 0)
            {
                return BadRequest(new ApiErrorResponse
                {
                    StatusCode = StatusCodes.Status400BadRequest,
                    Message = "UserId must be a positive integer."
                });
            }

            var user = await _userService.GetUserByIdAsync(userId);
            if (user == null)
            {
                return NotFound(new ApiErrorResponse
                {
                    StatusCode = StatusCodes.Status404NotFound,
                    Message = $"User with ID {userId} not found."
                });
            }

            return Ok(user);
        }

        /// <summary>
        /// Creates a new user record.
        /// POST: /api/users
        /// </summary>
        [HttpPost]
        [ProducesResponseType(typeof(UserResponseDto), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status409Conflict)]
        public async Task<IActionResult> CreateUser([FromBody] CreateUserDto dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var (user, errorMessage) = await _userService.CreateUserAsync(dto);
            if (errorMessage != null)
            {
                return Conflict(new ApiErrorResponse
                {
                    StatusCode = StatusCodes.Status409Conflict,
                    Message = errorMessage
                });
            }

            return CreatedAtAction(nameof(GetUserById), new { userId = user!.UserId }, user);
        }

        /// <summary>
        /// Updates an existing user by UserId.
        /// PUT: /api/users/{userId}
        /// </summary>
        [HttpPut("{userId:int}")]
        [ProducesResponseType(typeof(UserResponseDto), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status409Conflict)]
        public async Task<IActionResult> UpdateUser(int userId, [FromBody] UpdateUserDto dto)
        {
            if (userId <= 0)
            {
                return BadRequest(new ApiErrorResponse
                {
                    StatusCode = StatusCodes.Status400BadRequest,
                    Message = "UserId must be a positive integer."
                });
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var (user, errorMessage, notFound) = await _userService.UpdateUserAsync(userId, dto);
            if (notFound)
            {
                return NotFound(new ApiErrorResponse
                {
                    StatusCode = StatusCodes.Status404NotFound,
                    Message = errorMessage ?? $"User with ID {userId} not found."
                });
            }

            if (errorMessage != null)
            {
                return Conflict(new ApiErrorResponse
                {
                    StatusCode = StatusCodes.Status409Conflict,
                    Message = errorMessage
                });
            }

            return Ok(user);
        }

        /// <summary>
        /// Deletes a user by UserId.
        /// DELETE: /api/users/{userId}
        /// </summary>
        [HttpDelete("{userId:int}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> DeleteUser(int userId)
        {
            if (userId <= 0)
            {
                return BadRequest(new ApiErrorResponse
                {
                    StatusCode = StatusCodes.Status400BadRequest,
                    Message = "UserId must be a positive integer."
                });
            }

            bool deleted = await _userService.DeleteUserAsync(userId);
            if (!deleted)
            {
                return NotFound(new ApiErrorResponse
                {
                    StatusCode = StatusCodes.Status404NotFound,
                    Message = $"User with ID {userId} not found."
                });
            }

            return NoContent();
        }
    }
}
