using Domain.Interfaces;
using Infrastructure.Data;
using System;
using System.Collections.Generic;
using System.Text;

namespace Infrastructure.Repositories
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly CourseContext _context;
        public ICourseRepository Courses { get; private set; }
        public IChapterRepository Chapters { get; private set; }
        public IUserRepository Users { get; private set; }
        public UnitOfWork(CourseContext context)
        {
            _context = context;
            Courses = new CourseRepository(_context);
            Users = new UserRepository(_context);
            Chapters = new ChapterRepository(_context);
        }

        public Task SaveChangesAsync()
        {
            return _context.SaveChangesAsync();
        }
    }
}
