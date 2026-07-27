using Domain.Interfaces;
using Domain.Models;
using Infrastructure.Data;
using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories
{
    public class ExamAttemptRepository : IExamAttemptRepository
    {
        private readonly CourseContext _context;
        public ExamAttemptRepository(CourseContext context)
        {
            _context = context;
        }

        public async Task AddAsync(ExamAttempt entity)
        {
            await _context.ExamAttempts.AddAsync(entity);
        }

        public async Task DeleteAsync(int id)
        {
            var examAttempt = await _context.ExamAttempts.FindAsync(id);
            if (examAttempt != null)
            {
                _context.ExamAttempts.Remove(examAttempt);
            }
        }

        public async Task<IEnumerable<ExamAttempt>> GetAllAsync()
        {
            return await _context.ExamAttempts.ToListAsync();
        }

        public async Task<ExamAttempt> GetByIdAsync(int id)
        {
            return await _context.ExamAttempts.FindAsync(id);
        }

        public void Update(ExamAttempt entity)
        {
            _context.ExamAttempts.Update(entity);
        }
    }
}
