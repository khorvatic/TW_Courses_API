using Application.DTO.Answer;
using Application.DTO.Question;
using Application.Interfaces;
using Domain.Exceptions;
using Domain.Interfaces;
using Domain.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace Application.Services
{
    public class QuestionService : IQuestionService
    {
        private readonly IUnitOfWork _unitOfWork;

        public QuestionService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<QuestionDto> CreateQuestionAsync(CreateQuestionDto dto)
        {
            var question = new Question
            {
                Text = dto.Text,
                Type = dto.Type,
                ExamId = dto.ExamId,
                Answers = dto.Answers.Select(a => new Answer
                {
                    Correct = a.Correct,
                    Option = a.Option
                }).ToList()
            };

            await _unitOfWork.Questions.AddAsync(question);
            await _unitOfWork.SaveChangesAsync();

            return new QuestionDto
            {
                Id = question.Id,
                Text = question.Text,
                Type = question.Type,
                ExamId = question.ExamId,
                Answers = question.Answers.Select(a => new AnswerDto
                {
                    Id = a.Id,
                    QuestionId = a.QuestionId,
                    Option = a.Option
                }).ToList()
            };
        }

        public async Task DeleteQuestionAsync(int id)
        {
            var question = await _unitOfWork.Questions.GetByIdAsync(id);
            if (question == null)
                throw new NotFoundException("Cannot delete because Question with that ID not found");

            await _unitOfWork.Questions.DeleteAsync(id);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task<IEnumerable<QuestionDto>> GetAllQuestionsAsync()
        {
            var questions = await _unitOfWork.Questions.GetAllAsync();
            if (questions == null) throw new NotFoundException("No Questions were found");

            return questions.Select(q => new QuestionDto
            {
                Id = q.Id,
                Text = q.Text,
                Type = q.Type,
                ExamId = q.ExamId,
                Answers = q.Answers.Select(a => new AnswerDto
                {
                    Id = a.Id,
                    QuestionId = a.QuestionId,
                    Option = a.Option
                }).ToList()
            });
        }

        public async Task<QuestionDto> GetQuesitonByText(string text)
        {
            var question = await _unitOfWork.Questions.GetByTextAsync(text);
            if (question == null) throw new NotFoundException("Question with that text not found");

            return new QuestionDto
            {
                Id = question.Id,
                Text = question.Text,
                Type = question.Type,
                ExamId = question.ExamId,
                Answers = question.Answers.Select(a => new AnswerDto
                {
                    Id = a.Id,
                    QuestionId = a.QuestionId,
                    Option = a.Option
                }).ToList()
            };
        }

        public async Task<QuestionDto> GetQuestionByIdAsync(int id)
        {
            var question = await _unitOfWork.Questions.GetByIdAsync(id);
            if ( question == null)
                throw new NotFoundException("Question with that ID not found");

            return new QuestionDto
            {
                Id = question.Id,
                Text = question.Text,
                Type = question.Type,
                ExamId = question.ExamId,
                Answers = question.Answers.Select(a => new AnswerDto
                {
                    Id = a.Id,
                    QuestionId = a.QuestionId,
                    Option = a.Option
                }).ToList()
            };
        }

        public async Task<IEnumerable<QuestionDto>> GetQuestionsByExamIdAsync(int examId)
        {
            var questions = await _unitOfWork.Questions.GetByExamIdAsync(examId);
            if (questions == null)
                throw new NotFoundException("No Questions found for the given examId");

            return questions.Select(q => new QuestionDto
            {
                Id = q.Id,
                Text = q.Text,
                Type = q.Type,
                ExamId = q.ExamId,
                Answers = q.Answers.Select(a => new AnswerDto
                {
                    Id = a.Id,
                    QuestionId = a.QuestionId,
                    Option = a.Option
                }).ToList()
            });
        }

        public async Task<IEnumerable<QuestionDto>> GetQuestionsByTypeAsync(QuestionType type)
        {
            var questions = await _unitOfWork.Questions.GetByTypeAsync(type);
            if (questions == null)
                throw new NotFoundException("No questions found for the given questionType");

            return questions.Select(q => new QuestionDto
            {
                Id = q.Id,
                Text= q.Text,
                Type = q.Type,
                ExamId = q.ExamId,
                Answers = q.Answers.Select(a => new AnswerDto
                {
                    Id = a.Id,
                    QuestionId = a.QuestionId,
                    Option = a.Option
                }).ToList()
            });
        }

        public async Task<QuestionDto> UpdateQuestionAsync(int id, CreateQuestionDto dto)
        {
            var question = await _unitOfWork.Questions.GetByIdAsync(id);
            if (question == null)
                throw new NotFoundException("Cannot update because Question with that ID not found");

            question.Text = dto.Text;
            question.Type = dto.Type;
            question.ExamId = dto.ExamId;
            question.Answers = dto.Answers.Select(a => new Answer
            {
                Correct = a.Correct,
                Option = a.Option
            }).ToList();

            _unitOfWork.Questions.Update(question);
            await _unitOfWork.SaveChangesAsync();

            return new QuestionDto
            {
                Id = question.Id,
                Text = question.Text,
                Type = question.Type,
                ExamId = question.ExamId,
                Answers = question.Answers.Select(a => new AnswerDto
                {
                    Id = a.Id,
                    QuestionId = a.QuestionId,
                    Option = a.Option
                }).ToList()
            };
        }
    }
}