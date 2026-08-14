using Application.DTO.Role;
using Application.Interfaces;
using Domain.Interfaces;
using Domain.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Services
{
    public class RoleService : IRoleService
    {
        private readonly IUnitOfWork _unitOfWork;

        public RoleService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<RoleDto> CreateRoleAsync(CreateRoleDto dto)
        {
            var existingRole = await _unitOfWork.Roles.GetRoleByNameAsync(dto.Name);
            if (existingRole != null) throw new ArgumentException("Role with the same name already exists.");

            var role = new Role
            {
                Name = dto.Name
            };

            await _unitOfWork.Roles.AddAsync(role);
            await _unitOfWork.SaveChangesAsync();

            return new RoleDto
            {
                Id = role.Id,
                Name = role.Name
            };
        }

        public async Task DeleteRoleByIdAsync(int id)
        {
            var role = await _unitOfWork.Roles.GetByIdAsync(id);
            if (role != null)
            {
                await _unitOfWork.Roles.DeleteAsync(id);
                await _unitOfWork.SaveChangesAsync();
            }
        }

        public async Task<IEnumerable<RoleDto>> GetAllRolesAsync()
        {
            var roles = await _unitOfWork.Roles.GetAllAsync();
            return roles.Select(r => new RoleDto { Id = r.Id, Name = r.Name });
        }

        public async Task<RoleDto> GetRoleByIdAsync(int id)
        {
            var role = await _unitOfWork.Roles.GetByIdAsync(id);
            if (role != null)
            {
                return new RoleDto { Id = role.Id, Name = role.Name };
            }
            throw new ArgumentException("Role not found.");
        }

        public async Task<RoleDto> GetRoleByNameAsync(string name)
        {
            var role = await _unitOfWork.Roles.GetRoleByNameAsync(name);
            if (role != null)
            {
                return new RoleDto { Id = role.Id, Name = role.Name };
            }
            throw new ArgumentException("Role not found.");
        }
    }
}
