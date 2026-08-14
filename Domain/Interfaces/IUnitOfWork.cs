using System;
using System.Collections.Generic;
using System.Text;

namespace Domain.Interfaces
{
    public interface IUnitOfWork
    {
        public IAnswerRepository Answers { get; }
        public ICourseRepository Courses { get; }
        public IChapterRepository Chapters { get; }
        public IEnrolledCourseRepository EnrolledCourses { get; }
        public IExamAttemptRepository ExamAttempts { get; }
        public IExamQuestionAnswerRepository ExamQuestionAnswers { get; }
        public IExamRepository Exams { get; }
        public IQuestionRepository Questions { get; }
        public IReviewRepository Reviews { get; }
        public IRoleRepository Roles { get; }
        public IUserRepository Users { get; }
        public IUserRoleRepository UserRoles { get; }
        Task SaveChangesAsync();
    }
}
