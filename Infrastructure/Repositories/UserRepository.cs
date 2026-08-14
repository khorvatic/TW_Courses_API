using Domain.Interfaces;
using Domain.Models;
using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.EntityFrameworkCore;
using Infrastructure.Data;

namespace Infrastructure.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly CourseContext _context;

        public UserRepository(CourseContext context)
        {
            _context = context;
        }

        public async Task AddAsync(User entity)
        {
            await _context.Users.AddAsync(entity);
        }

        public async Task DeleteAsync(int id)
        {
            var user = await _context.Users
                .Include(u => u.Reviews)
                .FirstOrDefaultAsync(u => u.Id == id);
            if (user != null)
            {
                _context.Users.Remove(user);
            }
        }

        public async Task<IEnumerable<User>> GetAllAsync()
        {
            return await _context.Users
                .Include(u => u.Reviews)
                .ToListAsync();
        }

        public async Task<User> GetByIdAsync(int id)
        {
            return await _context.Users
                .Include(u => u.Reviews)
                .FirstOrDefaultAsync(u => u.Id == id);
        }

        public async Task<User> GetUserByEmailAsync(string email)
        {
            return await _context.Users
                .Include(u => u.Reviews)
                .FirstOrDefaultAsync(u => u.Email == email);
        }

        public async Task<IEnumerable<User>> GetUsersByFullNameAsync(string name, string surname)
        {
            return await _context.Users
                .Include(u => u.Reviews)
                .Where(u => u.Name == name && u.Surname == surname)
                .ToListAsync();
        }

        public async Task<IEnumerable<User>> GetUsersByNameAsync(string name)
        {
            return await _context.Users
                .Include(u => u.Reviews)
                .Where(u => u.Name == name)
                .ToListAsync();
        }

        public async Task<IEnumerable<User>> GetUsersBySurnameAsync(string surname)
        {
            return await _context.Users
                .Include(u => u.Reviews)
                .Where(u => u.Surname == surname)
                .ToListAsync();
        }

        public void Update(User entity)
        {
            _context.Users.Update(entity);
        }
    }
}
