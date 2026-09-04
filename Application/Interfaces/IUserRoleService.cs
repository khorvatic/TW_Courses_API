using Application.DTO.UserRole;
using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Interfaces
{
    public interface IUserRoleService
    {
        Task<IEnumerable<UserRoleDto>> GetAllUserRolesAsync();
        Task<IEnumerable<UserRoleDto>> GetUserRolesByUserIdAsync(int userId);
        Task<IEnumerable<UserRoleDto>> GetUserRolesByRoleIdAsync(int roleId);
        Task<UserRoleDto> GetByCompositeIdAsync(int userId, int roleId);
        Task<UserRoleDto> CreateUserRoleAsync(UpdateUserRoleDto createUserRoleDto);
        Task DeleteUserRoleAsync(int userId, int roleId);
    }
}
