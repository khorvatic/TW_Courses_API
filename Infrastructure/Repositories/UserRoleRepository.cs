using Domain.Interfaces;
using Domain.Models;
using Infrastructure.Data;
using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories
{
    public class UserRoleRepository : IUserRoleRepository
    {
        private readonly CourseContext _context;

        public UserRoleRepository(CourseContext context)
        {
            _context = context;
        }

        public async Task AddAsync(UserRole entity)
        {
            await _context.UserRoles.AddAsync(entity);
        }

        public async Task DeleteAsync(int id)
        {
            var userRole = await _context.UserRoles.FindAsync(id);
            if (userRole != null)
            {
                _context.UserRoles.Remove(userRole);
            }
        }

        public async Task DeleteCompositeAsync(int userId, int roleId)
        {
            var userRole = await _context.UserRoles.FirstOrDefaultAsync(ur => ur.UserId == userId && ur.RoleId == roleId);
            if (userRole != null)
            {
                _context.UserRoles.Remove(userRole);
            }
        }

        public async Task<IEnumerable<UserRole>> GetAllAsync()
        {
            return await _context.UserRoles.ToListAsync();
        }

        public async Task<UserRole> GetByCompositeIdAsync(int userId, int roleId)
        {
            return await _context.UserRoles.FirstOrDefaultAsync(ur => ur.UserId == userId && ur.RoleId == roleId);
        }

        public async Task<UserRole> GetByIdAsync(int id)
        {
            return await _context.UserRoles.FindAsync(id);
        }

        public async Task<IEnumerable<UserRole>> GetByRoleIdAsync(int roleId)
        {
            return await _context.UserRoles.Where(ur => ur.RoleId == roleId).ToListAsync();
        }

        public async Task<IEnumerable<UserRole>> GetByUserIdAsync(int userId)
        {
            return await _context.UserRoles.Where(ur => ur.UserId == userId).ToListAsync();
        }

        public void Update(UserRole entity)
        {
            _context.UserRoles.Update(entity);
        }
    }
}
