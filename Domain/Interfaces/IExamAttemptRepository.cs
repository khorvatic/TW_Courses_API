using Domain.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace Domain.Interfaces
{
    public interface IExamAttemptRepository : IGenericRepository<ExamAttempt>
    {
        Task<IEnumerable<ExamAttempt>> GetByExamIdAsync(int examId);
        Task<IEnumerable<ExamAttempt>> GetByUserIdAsync(int userId);
    }
}
