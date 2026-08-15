using Domain.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace Domain.Interfaces
{
    public interface IUserRoleRepository : IGenericRepository<UserRole>
    {
        Task DeleteCompositeAsync(int userId, int roleId);
        Task<UserRole> GetByCompositeIdAsync(int userId, int roleId);
        Task<IEnumerable<UserRole>> GetByRoleIdAsync(int roleId);
        Task<IEnumerable<UserRole>> GetByUserIdAsync(int userId);
    }
}
