using Domain.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace Domain.Interfaces
{
    public interface IUserRepository : IGenericRepository<User>
    {
        Task<User> GetUserByEmailAsync(string email);
        Task<IEnumerable<User>> GetUsersByFullNameAsync(string name, string surname);
        Task<IEnumerable<User>> GetUsersByNameAsync(string name);
        Task<IEnumerable<User>> GetUsersBySurnameAsync(string surname);
    }
}
