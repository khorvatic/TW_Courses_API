using Application.DTO.Answer;
using Application.DTO.Exam;
using Application.DTO.Question;
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
            if (exam != null) throw new ArgumentException("Exam with the same title already exists.");

            exam = new Exam
            {
                Title = dto.Title,
                AllotedTime = dto.AllotedTime,
                CourseId = dto.CourseId,
                Questions = dto.Questions.Select(q => new Question
                {
                    Type = q.Type,
                    ExamId = q.ExamId,
                    Answers = q.Answers.Select(a => new Answer
                    {
                        Option = a.Option,
                        Correct = a.Correct,
                        QuestionId = a.QuestionId,
                    }).ToList()
                }).ToList()
            };

            await _unitOfWork.Exams.AddAsync(exam);
            await _unitOfWork.SaveChangesAsync();

            return new ExamDto
            {
                Id = exam.Id,
                Title = exam.Title,
                AllotedTime = exam.AllotedTime,
                CourseId = exam.CourseId,
                Questions = exam.Questions.Select(q => new QuestionDto
                {
                    Id = q.Id,
                    Type = q.Type,
                    ExamId = q.ExamId,
                    Answers = q.Answers.Select(a => new AnswerDto
                    {
                        Id = a.Id,
                        Option = a.Option,
                        QuestionId = a.QuestionId,
                    }).ToList()
                }).ToList()
            };
        }

        public async Task<IEnumerable<ExamDto>> GetAllExamsAsync()
        {
            var exams = await _unitOfWork.Exams.GetAllAsync();

            return exams.Select(x => new ExamDto {
                Id = x.Id,
                Title = x.Title,
                AllotedTime = x.AllotedTime,
                CourseId = x.CourseId,
                Questions = x.Questions.Select(q => new QuestionDto
                {
                    Id = q.Id,
                    Type = q.Type,
                    ExamId = q.ExamId,
                    Answers = q.Answers.Select(a => new AnswerDto
                    {
                        Id = a.Id,
                        Option = a.Option,
                        QuestionId = a.QuestionId,
                    }).ToList()
                }).ToList()
            });
        }

        public async Task<ExamDto> GetExamByIdAsync(int id)
        {
            var exam = await _unitOfWork.Exams.GetByIdAsync(id);
            if (exam == null) throw new ArgumentException("Exam with that ID not found.");

            return new ExamDto
            {
                Id = exam.Id,
                Title = exam.Title,
                AllotedTime = exam.AllotedTime,
                CourseId = exam.CourseId,
                Questions = exam.Questions.Select(q => new QuestionDto
                {
                    Id = q.Id,
                    Type = q.Type,
                    ExamId = q.ExamId,
                    Answers = q.Answers.Select(a => new AnswerDto
                    {
                        Id = a.Id,
                        Option = a.Option,
                        QuestionId = a.QuestionId,
                    }).ToList()
                }).ToList()
            };
        }

        public async Task<IEnumerable<ExamDto>> GetExamsByCourseIdAsync(int courseId)
        {
            var exams = await _unitOfWork.Exams.GetByCourseIdAsync(courseId);
            if (exams == null || !exams.Any()) 
                throw new ArgumentException("No exams found for that course.");

            return exams.Select(x => new ExamDto
            {
                Id = x.Id,
                Title = x.Title,
                AllotedTime = x.AllotedTime,
                CourseId = x.CourseId,
                Questions = x.Questions.Select(q => new QuestionDto
                {
                    Id = q.Id,
                    Type = q.Type,
                    ExamId = q.ExamId,
                    Answers = q.Answers.Select(a => new AnswerDto
                    {
                        Id = a.Id,
                        Option = a.Option,
                        QuestionId = a.QuestionId,
                    }).ToList()
                }).ToList()
            });
        }

        public async Task RemoveExamAsync(int id)
        {
            var exam = await _unitOfWork.Exams.GetByIdAsync(id);
            if (exam == null) throw new ArgumentException("Exam with that ID not found.");

            await _unitOfWork.Exams.DeleteAsync(id);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task<ExamDto> UpdateExamAsync(int id, CreateExamDto dto)
        {
            var exam = await _unitOfWork.Exams.GetByIdAsync(id);
            if (exam == null) throw new ArgumentException("Exam with that ID not found.");

            exam.Title = dto.Title;
            exam.AllotedTime = dto.AllotedTime;
            exam.CourseId = dto.CourseId;
            exam.Questions = dto.Questions.Select(q => new Question
            {
                Type = q.Type,
                ExamId = q.ExamId,
                Answers = q.Answers.Select(a => new Answer
                {
                    Option = a.Option,
                    Correct = a.Correct,
                    QuestionId = a.QuestionId,
                }).ToList()
            }).ToList();

            _unitOfWork.Exams.Update(exam);
            await _unitOfWork.SaveChangesAsync();

            return new ExamDto
            {
                Id = exam.Id,
                Title = exam.Title,
                AllotedTime = exam.AllotedTime,
                CourseId = exam.CourseId,
                Questions = exam.Questions.Select(q => new QuestionDto
                {
                    Id = q.Id,
                    Type = q.Type,
                    ExamId = q.ExamId,
                    Answers = q.Answers.Select(a => new AnswerDto
                    {
                        Id = a.Id,
                        Option = a.Option,
                        QuestionId = a.QuestionId,
                    }).ToList()
                }).ToList()
            };
        }
    }
}