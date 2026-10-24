using System.Collections.Generic;
using System.Threading.Tasks;
using AspNetUserApi.DTOs;

namespace AspNetUserApi.Services
{
    public interface IUserService
    {
        Task<IEnumerable<UserResponseDto>> GetAllUsersAsync();
        Task<UserResponseDto?> GetUserByIdAsync(int userId);
        Task<(UserResponseDto? User, string? ErrorMessage)> CreateUserAsync(CreateUserDto dto);
        Task<(UserResponseDto? User, string? ErrorMessage, bool NotFound)> UpdateUserAsync(int userId, UpdateUserDto dto);
        Task<bool> DeleteUserAsync(int userId);
    }
}
