using Domain.Interfaces;
using Domain.Models;
using Infrastructure.Data;
using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories
{
    public class ExamRepository : IExamRepository
    {
        private readonly CourseContext _context;
        public ExamRepository(CourseContext context)
        {
            _context = context;
        }

        public async Task AddAsync(Exam entity)
        {
            await _context.Exams.AddAsync(entity);
        }

        public async Task DeleteAsync(int id)
        {
            var exam = await _context.Exams.FindAsync(id);
            if (exam != null)
            {
                _context.Exams.Remove(exam);
            }
        }

        public async Task<IEnumerable<Exam>> GetAllAsync()
        {
            return await _context.Exams
                .Include(e => e.Questions)
                .ToListAsync();
        }

        public async Task<IEnumerable<Exam>> GetByCourseIdAsync(int courseId)
        {
            return await _context.Exams
                .Include(e => e.Questions)
                .Where(e => e.CourseId == courseId)
                .ToListAsync();
        }

        public async Task<Exam> GetByIdAsync(int id)
        {
            return await _context.Exams
                .Include(e => e.Questions)
                .FirstOrDefaultAsync(e => e.Id == id);
        }

        public async Task<Exam> GetByTitleAsync(string title)
        {
            return await _context.Exams
                .Include(e => e.Questions)
                .FirstOrDefaultAsync(e => e.Title == title);
        }

        public void Update(Exam entity)
        {
            _context.Exams.Update(entity);
        }
    }
}
