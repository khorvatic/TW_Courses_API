using Application.DTO.Exam;
using Application.Interfaces;
using Domain.Interfaces;
using Domain.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Services
{
    public class ExamService : IExamService
    {
        private readonly IUnitOfWork _unitOfWork;

        public ExamService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<ExamDto> CreateExamAsync(CreateExamDto dto)
        {
            var exam = await _unitOfWork.Exams.GetByTitleAsync(dto.Title);
            if (exam == null) throw new ArgumentException("Exam with the same title already exists.");

            var newExam = new Exam
            {
                Title = dto.Title,
                AllotedTime = dto.AllotedTime,
                CourseId = dto.CourseId,
                Questions = dto.Questions....
            };
        }

        public async Task<IEnumerable<ExamDto>> GetAllExamsAsync()
        {
            var exams = await _unitOfWork.Exams.GetAllAsync();

            return exams.Select
        }

        public Task<ExamDto> GetExamByIdAsync(int id)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<ExamDto>> GetExamsByCourseIdAsync(int courseId)
        {
            throw new NotImplementedException();
        }

        public Task RemoveExamAsync(int id)
        {
            throw new NotImplementedException();
        }

        public Task<ExamDto> UpdateExamAsync(int id, CreateExamDto dto)
        {
            throw new NotImplementedException();
        }
    }
}
