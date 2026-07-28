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
        public IAnswerRepository Answers { get; private set; }
        public ICourseRepository Courses { get; private set; }
        public IChapterRepository Chapters { get; private set; }
        public IEnrolledCourseRepository Enrolled { get; private set; }
        public IExamAttemptRepository ExamAttempts { get; private set; }
        public IExamQuestionAnswerRepository ExamQuestionAnswers { get; private set; }
        public IExamRepository Exams { get; private set; }
        public IQuestionRepository Questions { get; private set; }
        public IReviewRepository Reviews { get; private set; }
        public IRoleRepository Roles { get; private set; }
        public IUserRoleRepository UserRoles { get; private set; }
        public IUserRepository Users { get; private set; }
        public UnitOfWork(CourseContext context)
        {
            _context = context;
            Courses = new CourseRepository(_context);
            Users = new UserRepository(_context);
            Chapters = new ChapterRepository(_context);
            Answers = new AnswerRepository(_context);
            Enrolled = new EnrolledCourseRepository(_context);
            ExamAttempts = new ExamAttemptRepository(_context);
            ExamQuestionAnswers = new ExamQuestionAnswerRepository(_context);
            Exams = new ExamRepository(_context);
            Questions = new QuestionRepository(_context);
            Reviews = new ReviewRepository(_context);
            Roles = new RoleRepository(_context);
            UserRoles = new UserRoleRepository(_context);
        }

        public Task SaveChangesAsync()
        {
            return _context.SaveChangesAsync();
        }
    }
}
