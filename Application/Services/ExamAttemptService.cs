using Application.DTO.ExamAttempt;
using Application.Interfaces;
using Domain.Exceptions;
using Domain.Interfaces;
using Domain.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Services
{
    public class ExamAttemptService : IExamAttemptService
    {
        private readonly IUnitOfWork _unitOfWork;

        public ExamAttemptService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<ExamAttemptDto> CreateExamAttemptAsync(CreateExamAttemptDto createExamAttemptDto)
        {
            var examAttempt = new ExamAttempt
            {
                ExamId = createExamAttemptDto.ExamId,
                UserId = createExamAttemptDto.UserId,
                Passed = false,
                Score = 0.0
            };

            await _unitOfWork.ExamAttempts.AddAsync(examAttempt);
            await _unitOfWork.SaveChangesAsync();

            return new ExamAttemptDto
            {
                Id = examAttempt.Id,
                ExamId = examAttempt.ExamId,
                UserId = examAttempt.UserId,
                Passed = examAttempt.Passed,
                Score = examAttempt.Score
            };
        }

        public async Task DeleteExamAttemptAsync(int id)
        {
            var ea = await _unitOfWork.ExamAttempts.GetByIdAsync(id);
            if (ea ==null) throw new NotFoundException("Cannot delete because Exam attempt with that ID not found");

            await _unitOfWork.ExamAttempts.DeleteAsync(id);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task<IEnumerable<ExamAttemptDto>> GetAllExamAttemptsAsync()
        {
            var examAttempts = await _unitOfWork.ExamAttempts.GetAllAsync();
            if (examAttempts == null) throw new NotFoundException("No Exam attempts were found");

            return examAttempts.Select(ea => new ExamAttemptDto
            {
                Id = ea.Id,
                ExamId = ea.ExamId,
                UserId = ea.UserId,
                Passed = ea.Passed,
                Score = ea.Score
            });
        }

        public async Task<ExamAttemptDto> GetExamAttemptByIdAsync(int id)
        {
            var ea = await _unitOfWork.ExamAttempts.GetByIdAsync(id);
            if (ea == null) throw new ArgumentException("Exam attempt with that ID not found");

            return new ExamAttemptDto
            {
                Id = ea.Id,
                ExamId = ea.ExamId,
                UserId = ea.UserId,
                Passed = ea.Passed,
                Score = ea.Score
            };
        }

        public async Task<IEnumerable<ExamAttemptDto>> GetExamAttemptsByExamIdAsync(int examId)
        {
            var examAttempts = await _unitOfWork.ExamAttempts.GetByExamIdAsync(examId);
            if (examAttempts == null || !examAttempts.Any()) throw new NotFoundException("No Exam attempts found for the specified Exam");

            return examAttempts.Select(ea => new ExamAttemptDto
            {
                Id = ea.Id,
                ExamId = ea.ExamId,
                UserId = ea.UserId,
                Passed = ea.Passed,
                Score = ea.Score
            });
        }

        public async Task<IEnumerable<ExamAttemptDto>> GetExamAttemptsByUserIdAsync(int userId)
        {
            var examAttempts = await _unitOfWork.ExamAttempts.GetByUserIdAsync(userId);
            if (examAttempts == null || !examAttempts.Any()) throw new NotFoundException("No Exam attempts found for the specified User");

            return examAttempts.Select(ea => new ExamAttemptDto
            {
                Id = ea.Id,
                ExamId = ea.ExamId,
                UserId = ea.UserId,
                Passed = ea.Passed,
                Score = ea.Score
            });
        }

        public async Task<ExamAttemptDto> SubmitExamAttemptAsync(int attemptId)
        {
            var attempt = await _unitOfWork.ExamAttempts.GetByIdAsync(attemptId);
            if (attempt == null) throw new NotFoundException("Exam attempt with that ID not found");

            var eqas = await _unitOfWork.ExamQuestionAnswers.GetAllForAttemptIdAsync(attemptId);
            var correctAnswers = eqas.Count(e => e.Answer.Correct);

            double score = 0.0;

            if (eqas.Any())
            {
                score = (double)correctAnswers / eqas.Count() * 100;
            }

            attempt.Score = score;
            attempt.Passed = score >= 50;

            _unitOfWork.ExamAttempts.Update(attempt);
            await _unitOfWork.SaveChangesAsync();

            return new ExamAttemptDto
            {
                Id = attempt.Id,
                ExamId = attempt.ExamId,
                UserId = attempt.UserId,
                Passed = attempt.Passed,
                Score = attempt.Score
            };
        }
    }
}
