using Application.DTO.UserRole;
using Application.Interfaces;
using Domain.Exceptions;
using Domain.Interfaces;
using Domain.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Services
{
    public class UserRoleService : IUserRoleService
    {
        private readonly IUnitOfWork _unitOfWork;

        public UserRoleService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<UserRoleDto> CreateUserRoleAsync(CreateUserRoleDto createUserRoleDto)
        {
            var userRole = await _unitOfWork.UserRoles.GetByCompositeIdAsync(createUserRoleDto.UserId, createUserRoleDto.RoleId);
            if (userRole != null)
                throw new BusinessRuleException("User role already exists");
            
            userRole = new UserRole
            {
                UserId = createUserRoleDto.UserId,
                RoleId = createUserRoleDto.RoleId
            };

            await _unitOfWork.UserRoles.AddAsync(userRole);
            await _unitOfWork.SaveChangesAsync();

            return new UserRoleDto
            {
                UserId = userRole.UserId,
                RoleId = userRole.RoleId
            };
        }

        public async Task DeleteUserRoleAsync(int userId, int roleId)
        {
            var userRole = await _unitOfWork.UserRoles.GetByCompositeIdAsync(userId, roleId);
            if (userRole == null)
                throw new NotFoundException("Cannot delete becuaes UserRole with that ID not found");

            await _unitOfWork.UserRoles.DeleteCompositeAsync(userId, roleId);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task<IEnumerable<UserRoleDto>> GetAllUserRolesAsync()
        {
            var userRoles = await _unitOfWork.UserRoles.GetAllAsync();

            return userRoles.Select(ur => new UserRoleDto
            {
                UserId = ur.UserId,
                RoleId = ur.RoleId
            });
        }

        public async Task<UserRoleDto> GetByCompositeId(int userId, int roleId)
        {
            var userRole = await _unitOfWork.UserRoles.GetByCompositeIdAsync(userId, roleId);
            if (userRole == null) throw new NotFoundException("UserRole with that ID not found");

            return new UserRoleDto
            {
                UserId = userRole.UserId,
                RoleId = userRole.RoleId
            };
        }

        public async Task<IEnumerable<UserRoleDto>> GetUserRolesByRoleIdAsync(int roleId)
        {
            var userRoles = await _unitOfWork.UserRoles.GetByRoleIdAsync(roleId);
            if (userRoles == null) throw new NotFoundException("No UserRoles found for the given roleId");

            return userRoles.Select(ur => new UserRoleDto
            {
                UserId = ur.UserId,
                RoleId = ur.RoleId
            });
        }

        public async Task<IEnumerable<UserRoleDto>> GetUserRolesByUserIdAsync(int userId)
        {
            var userRoles = await _unitOfWork.UserRoles.GetByUserIdAsync(userId);
            if (userRoles == null) throw new NotFoundException("No UserRoles found for the given userId");

            return userRoles.Select(ur => new UserRoleDto
            {
                UserId = ur.UserId,
                RoleId = ur.RoleId
            });
        }
    }
}
