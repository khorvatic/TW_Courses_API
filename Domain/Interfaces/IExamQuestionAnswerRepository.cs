using Domain.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace Domain.Interfaces
{
    public interface IExamQuestionAnswerRepository : IGenericRepository<ExamQuestionAnswer>
    {
        Task DeleteCompositeAsync(int answerId, int questionId, int attemptId);
        Task<ExamQuestionAnswer> GetByCompositeIdAsync(int answerId, int questionId, int attemptId);
        Task<IEnumerable<ExamQuestionAnswer>> GetAllForAnswerIdAsync(int answerId);
        Task<IEnumerable<ExamQuestionAnswer>> GetAllForQuestionIdAsync(int questionId);
        Task<IEnumerable<ExamQuestionAnswer>> GetAllForAttemptIdAsync(int attemptId);
    }
}
