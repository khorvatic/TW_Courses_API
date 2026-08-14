using Application.DTO.ExamQuestionAnswer;
using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Interfaces
{
    public interface IExamQuestionAnswerService
    {
        Task<ExamQuestionAnswerDto> CreateExamQuestionAnswerAsync(CreateExamQuestionAnswerDto dto);
        Task<IEnumerable<ExamQuestionAnswerDto>> GetExamQuestionAnswerByAnswerIdAsync(int answerId);
        Task<IEnumerable<ExamQuestionAnswerDto>> GetExamQuestionAnswerByQuestionIdAsync(int questionId);
        Task<IEnumerable<ExamQuestionAnswerDto>> GetExamQuestionAnswerByAttemptIdAsync(int attemptId);
        Task<IEnumerable<ExamQuestionAnswerDto>> GetAllExamQuestionAnswersAsync();
        Task DeleteExamQuestionAnswerAsync(int answerId, int questionId, int attemptId);
    }
}
