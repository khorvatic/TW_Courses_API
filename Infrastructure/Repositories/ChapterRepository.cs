using Domain.Interfaces;
using Domain.Models;
using Infrastructure.Data;
using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories
{
    public class ChapterRepository : IChapterRepository
    {
        private readonly CourseContext _context;

        public ChapterRepository(CourseContext context)
        {
            _context = context;
        }

        public async Task AddAsync(Chapter entity)
        {
            await _context.Chapters.AddAsync(entity);
        }

        public async Task DeleteAsync(int id)
        {
            var chapter = await _context.Chapters.FindAsync(id);
            if (chapter != null)
            {
                _context.Chapters.Remove(chapter);
            }
        }

        public async Task<IEnumerable<Chapter>> GetAllAsync()
        {
            return await _context.Chapters.ToListAsync();
        }

        public async Task<IEnumerable<Chapter>> GetByCourseIdAsync(int courseId)
        {
            return await _context.Chapters.Where(c => c.CourseId == courseId).ToListAsync();
        }

        public async Task<Chapter> GetByIdAsync(int id)
        {
            return await _context.Chapters.FindAsync(id);
        }

        public void Update(Chapter entity)
        {
            _context.Chapters.Update(entity);
        }
    }
}
