using Domain.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace Domain.Interfaces
{
    public interface IQuestionRepository : IGenericRepository<Question>
    {
        Task<IEnumerable<Question>> GetByExamIdAsync(int examId);
        Task<IEnumerable<Question>> GetByTypeAsync(QuestionType type);
        Task<Question> GetByTextAsync(string text);
    }
}
