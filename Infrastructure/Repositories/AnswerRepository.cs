using Domain.Interfaces;
using Domain.Models;
using Infrastructure.Data;
using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories
{
    public class AnswerRepository : IAnswerRepository
    {
        private readonly CourseContext _context;
        public AnswerRepository(CourseContext courseContext)
        {
            _context = courseContext;
        }
        public async Task AddAsync(Answer entity)
        {
            await _context.Answers.AddAsync(entity);
        }

        public async Task DeleteAsync(int id)
        {
            var answer = await _context.Answers.FindAsync(id);
            if (answer != null)
            {
                _context.Answers.Remove(answer);
            }
        }

        public async Task<IEnumerable<Answer>> GetAllAsync()
        {
            return await _context.Answers.ToListAsync();
        }

        public async Task<Answer> GetByIdAsync(int id)
        {
            return await _context.Answers.FindAsync(id);
        }

        public void Update(Answer entity)
        {
            _context.Answers.Update(entity);
        }
    }
}
