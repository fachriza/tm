using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AspNetUserApi.Data;
using AspNetUserApi.DTOs;
using AspNetUserApi.Models;
using Microsoft.EntityFrameworkCore;

namespace AspNetUserApi.Services
{
    /// <summary>
    /// Implements user domain business logic, data access, and validation rules.
    /// </summary>
    public class UserService : IUserService
    {
        private readonly AppDbContext _context;

        public UserService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<UserResponseDto>> GetAllUsersAsync()
        {
            return await _context.Users
                .AsNoTracking()
                .OrderBy(u => u.UserId)
                .Select(u => MapToDto(u))
                .ToListAsync();
        }

        public async Task<UserResponseDto?> GetUserByIdAsync(int userId)
        {
            var user = await _context.Users
                .AsNoTracking()
                .FirstOrDefaultAsync(u => u.UserId == userId);

            return user == null ? null : MapToDto(user);
        }

        public async Task<(UserResponseDto? User, string? ErrorMessage)> CreateUserAsync(CreateUserDto dto)
        {
            // Business rule: Email must be unique across all users
            string normalizedEmail = dto.Email.Trim().ToLowerInvariant();
            bool emailExists = await _context.Users
                .AnyAsync(u => u.Email.ToLower() == normalizedEmail);

            if (emailExists)
            {
                return (null, $"User with email '{dto.Email}' already exists.");
            }

            var newUser = new User
            {
                FirstName = dto.FirstName.Trim(),
                LastName = dto.LastName.Trim(),
                Email = normalizedEmail,
                CreatedAt = DateTime.UtcNow
            };

            _context.Users.Add(newUser);
            await _context.SaveChangesAsync();

            return (MapToDto(newUser), null);
        }

        public async Task<(UserResponseDto? User, string? ErrorMessage, bool NotFound)> UpdateUserAsync(int userId, UpdateUserDto dto)
        {
            var user = await _context.Users.FindAsync(userId);
            if (user == null)
            {
                return (null, $"User with ID {userId} was not found.", true);
            }

            string normalizedEmail = dto.Email.Trim().ToLowerInvariant();

            // Business rule: Email must not conflict with any OTHER existing user
            bool emailConflict = await _context.Users
                .AnyAsync(u => u.Email.ToLower() == normalizedEmail && u.UserId != userId);

            if (emailConflict)
            {
                return (null, $"Email '{dto.Email}' is already in use by another user.", false);
            }

            user.FirstName = dto.FirstName.Trim();
            user.LastName = dto.LastName.Trim();
            user.Email = normalizedEmail;
            user.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            return (MapToDto(user), null, false);
        }

        public async Task<bool> DeleteUserAsync(int userId)
        {
            var user = await _context.Users.FindAsync(userId);
            if (user == null)
            {
                return false;
            }

            _context.Users.Remove(user);
            await _context.SaveChangesAsync();
            return true;
        }

        private static UserResponseDto MapToDto(User user)
        {
            return new UserResponseDto
            {
                UserId = user.UserId,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Email = user.Email,
                CreatedAt = user.CreatedAt,
                UpdatedAt = user.UpdatedAt
            };
        }
    }
}
