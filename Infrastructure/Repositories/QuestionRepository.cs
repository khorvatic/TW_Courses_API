using Domain.Interfaces;
using Domain.Models;
using Infrastructure.Data;
using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories
{
    public class QuestionRepository : IQuestionRepository
    {
        private readonly CourseContext _context;

        public QuestionRepository(CourseContext context)
        {
            _context = context;
        }

        public async Task AddAsync(Question entity)
        {
            await _context.Questions.AddAsync(entity);
        }

        public async Task DeleteAsync(int id)
        {
            var question = await _context.Questions.FindAsync(id);
            if (question != null)
            {
                _context.Questions.Remove(question);
            }
        }

        public async Task<IEnumerable<Question>> GetAllAsync()
        {
            return await _context.Questions
                .Include(q => q.Answers)
                .ToListAsync();
        }

        public async Task<IEnumerable<Question>> GetByExamIdAsync(int examId)
        {
            return await _context.Questions
                .Include(q => q.Answers)
                .Where(q => q.ExamId == examId)
                .ToListAsync();
        }

        public async Task<Question> GetByIdAsync(int id)
        {
            return await _context.Questions
                .Include(q => q.Answers)
                .FirstOrDefaultAsync(q => q.Id == id);
        }

        public async Task<IEnumerable<Question>> GetByTypeAsync(QuestionType type)
        {
            return await _context.Questions
                .Include(q => q.Answers)
                .Where(q => q.Type == type)
                .ToListAsync();
        }

        public void Update(Question entity)
        {
            _context.Questions.Update(entity);
        }
    }
}
