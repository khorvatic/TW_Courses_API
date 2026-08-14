using Application.DTO.User;
using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Interfaces
{
    public interface IUserService
    {
        Task<UserDto> CreateUserAsync(CreateUserDto dto);
        Task<UserDto> GetUserByIdAsync(int id);
        Task<IEnumerable<UserDto>> GetUsersByFullNameAsync(string name, string surname);
        Task<IEnumerable<UserDto>> GetAllUsersAsync();
        Task<IEnumerable<UserDto>> GetUsersByNameAsync(string name);
        Task<IEnumerable<UserDto>> GetUsersBySurnameAsync(string surname);
        Task<UserDto> UpdateUserAsync(int id, CreateUserDto dto);
        Task DeleteUserAsync(int id);
        Task<UserDto> GetUserByEmailAsync(string email);
    }
}
