using Application.DTO.ExamAttempt;
using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Interfaces
{
    public interface IExamAttemptService
    {
        Task<ExamAttemptDto> CreateExamAttemptAsync(CreateExamAttemptDto createExamAttemptDto);
        Task<ExamAttemptDto> GetExamAttemptByIdAsync(int id);
        Task<IEnumerable<ExamAttemptDto>> GetAllExamAttemptsAsync();
        Task<ExamAttemptDto> SubmitExamAttemptAsync(int attemptId);
        Task DeleteExamAttemptAsync(int id);
        Task<IEnumerable<ExamAttemptDto>> GetExamAttemptsByUserIdAsync(int userId);
        Task<IEnumerable<ExamAttemptDto>> GetExamAttemptsByExamIdAsync(int examId);
    }
}
