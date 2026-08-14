using Application.DTO.Exam;
using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Interfaces
{
    public interface IExamService
    {
        Task<IEnumerable<ExamDto>> GetAllExamsAsync();
        Task<IEnumerable<ExamDto>> GetExamsByCourseIdAsync(int courseId);
        Task<ExamDto> GetExamByIdAsync(int id);
        Task<ExamDto> CreateExamAsync(CreateExamDto dto);
        Task<ExamDto> UpdateExamAsync(int id, CreateExamDto dto);
        Task RemoveExamAsync(int id);
    }
}
