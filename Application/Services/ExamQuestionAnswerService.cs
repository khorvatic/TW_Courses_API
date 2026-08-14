using Application.DTO.ExamQuestionAnswer;
using Application.Interfaces;
using Domain.Interfaces;
using Domain.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Services
{
    public class ExamQuestionAnswerService : IExamQuestionAnswerService
    {
        private readonly IUnitOfWork _unitOfWork;

        public ExamQuestionAnswerService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<ExamQuestionAnswerDto> CreateExamQuestionAnswerAsync(CreateExamQuestionAnswerDto dto)
        {
            var eqa = await _unitOfWork.ExamQuestionAnswers.GetByCompositeIdAsync(dto.AnswerId, dto.QuestionId, dto.AttemptId);
            if (eqa != null) throw new ArgumentException("Exam question answer already exists");

            eqa = new ExamQuestionAnswer
            {
                AnswerId = dto.AnswerId,
                QuestionId = dto.QuestionId,
                AttemptId = dto.AttemptId
            };

            await _unitOfWork.ExamQuestionAnswers.AddAsync(eqa);
            await _unitOfWork.SaveChangesAsync();

            return new ExamQuestionAnswerDto
            {
                AnswerId = eqa.AnswerId,
                QuestionId = eqa.QuestionId,
                AttemptId = eqa.AttemptId
            };
        }

        public async Task DeleteExamQuestionAnswerAsync(int answerId, int questionId, int attemptId)
        {
            var eqa = await _unitOfWork.ExamQuestionAnswers.GetByCompositeIdAsync(answerId, questionId, attemptId);
            if (eqa == null) throw new ArgumentException("Exam question answer not found");

            await _unitOfWork.ExamQuestionAnswers.DeleteCompositeAsync(answerId, questionId, attemptId);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task<IEnumerable<ExamQuestionAnswerDto>> GetAllExamQuestionAnswersAsync()
        {
            var eqas = await _unitOfWork.ExamQuestionAnswers.GetAllAsync();

            return eqas.Select(eqa => new ExamQuestionAnswerDto
            {
                AnswerId = eqa.AnswerId,
                QuestionId = eqa.QuestionId,
                AttemptId = eqa.AttemptId
            });
        }

        public async Task<IEnumerable<ExamQuestionAnswerDto>> GetExamQuestionAnswerByAnswerIdAsync(int answerId)
        {
            var eqas = await _unitOfWork.ExamQuestionAnswers.GetAllForAnswerIdAsync(answerId);
            if (eqas == null) throw new ArgumentException("No exam question answers found for the given answerId");

            return eqas.Select(eqa => new ExamQuestionAnswerDto
            {
                AnswerId = eqa.AnswerId,
                QuestionId = eqa.QuestionId,
                AttemptId = eqa.AttemptId
            });
        }

        public async Task<IEnumerable<ExamQuestionAnswerDto>> GetExamQuestionAnswerByAttemptIdAsync(int attemptId)
        {
            var eqas = await _unitOfWork.ExamQuestionAnswers.GetAllForAttemptIdAsync(attemptId);
            if (eqas == null) throw new ArgumentException("No exam question answers found for the given attemptId");

            return eqas.Select(eqa => new ExamQuestionAnswerDto
            {
                AnswerId = eqa.AnswerId,
                QuestionId = eqa.QuestionId,
                AttemptId = eqa.AttemptId
            });
        }

        public async Task<IEnumerable<ExamQuestionAnswerDto>> GetExamQuestionAnswerByQuestionIdAsync(int questionId)
        {
            var eqas = await _unitOfWork.ExamQuestionAnswers.GetAllForQuestionIdAsync(questionId);
            if (eqas == null) throw new ArgumentException("No exam question answers found for the given questionId");

            return eqas.Select(eqa => new ExamQuestionAnswerDto
            {
                AnswerId = eqa.AnswerId,
                QuestionId = eqa.QuestionId,
                AttemptId = eqa.AttemptId
            });
        }
    }
}
