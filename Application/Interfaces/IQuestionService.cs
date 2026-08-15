using Application.DTO.Question;
using Domain.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Interfaces
{
    public interface IQuestionService
    {
        Task<IEnumerable<QuestionDto>> GetQuestionsByExamIdAsync(int examId);
        Task<IEnumerable<QuestionDto>> GetAllQuestionsAsync();
        Task<QuestionDto> GetQuestionByIdAsync(int id);
        Task<IEnumerable<QuestionDto>> GetQuestionsByTypeAsync(QuestionType type);
        Task<QuestionDto> CreateQuestionAsync(CreateQuestionDto dto);
        Task<QuestionDto> UpdateQuestionAsync(int id, CreateQuestionDto dto);
        Task DeleteQuestionAsync(int id);
    }
}
