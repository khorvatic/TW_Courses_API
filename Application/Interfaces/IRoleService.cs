using Application.DTO.Role;
using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Interfaces
{
    public interface IRoleService
    {
        Task<IEnumerable<RoleDto>> GetAllRolesAsync();
        Task<RoleDto> GetRoleByIdAsync(int id);
        Task<RoleDto> GetRoleByNameAsync(string name);
        Task DeleteRoleByIdAsync(int id);
        Task<RoleDto> CreateRoleAsync(CreateRoleDto dto);
    }
}
