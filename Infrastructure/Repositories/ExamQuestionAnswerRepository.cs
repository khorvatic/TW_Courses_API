using Domain.Interfaces;
using Domain.Models;
using Infrastructure.Data;
using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories
{
    public class ExamQuestionAnswerRepository : IExamQuestionAnswerRepository
    {
        private readonly CourseContext _context;
        public ExamQuestionAnswerRepository(CourseContext context)
        {
            _context = context;
        }

        public async Task AddAsync(ExamQuestionAnswer entity)
        {
            await _context.ExamQuestionAnswers.AddAsync(entity);
        }

        public async Task DeleteAsync(int id)
        {
            var examQuestionAnswer = await _context.ExamQuestionAnswers.FindAsync(id);
            if (examQuestionAnswer != null)
            {
                _context.ExamQuestionAnswers.Remove(examQuestionAnswer);
            }
        }

        public async Task DeleteCompositeAsync(int answerId, int questionId, int attemptId)
        {
            var examQuestionAnswer = await _context.ExamQuestionAnswers.FirstOrDefaultAsync(
                eqa => eqa.AnswerId == answerId && eqa.QuestionId == questionId && eqa.AttemptId == attemptId);
            if (examQuestionAnswer != null)
            {
                _context.ExamQuestionAnswers.Remove(examQuestionAnswer);
            }
        }

        public async Task<ExamQuestionAnswer> GetByCompositeIdAsync(int answerId, int questionId, int attemptId)
        {
            return await _context.ExamQuestionAnswers.FirstOrDefaultAsync(
                eqa => eqa.AnswerId == answerId && eqa.QuestionId == questionId && eqa.AttemptId == attemptId);
        }

        public async Task<IEnumerable<ExamQuestionAnswer>> GetAllAsync()
        {
            return await _context.ExamQuestionAnswers.ToListAsync();
        }

        public async Task<ExamQuestionAnswer> GetByIdAsync(int id)
        {
            return await _context.ExamQuestionAnswers.FindAsync(id);
        }

        public void Update(ExamQuestionAnswer entity)
        {
            _context.ExamQuestionAnswers.Update(entity);
        }

        public async Task<IEnumerable<ExamQuestionAnswer>> GetAllForAnswerIdAsync(int answerId)
        {
            return await _context.ExamQuestionAnswers
                .Where(eqa => eqa.AnswerId == answerId)
                .ToListAsync();
        }

        public async Task<IEnumerable<ExamQuestionAnswer>> GetAllForQuestionIdAsync(int questionId)
        {
            return await _context.ExamQuestionAnswers
                .Where(eqa => eqa.QuestionId == questionId)
                .ToListAsync();
        }

        public async Task<IEnumerable<ExamQuestionAnswer>> GetAllForAttemptIdAsync(int attemptId)
        {
            return await _context.ExamQuestionAnswers
                .Where(eqa => eqa.AttemptId == attemptId)
                .Include(eqa => eqa.Answer)
                .Include(eqa => eqa.Question)
                .ToListAsync();
        }
    }
}
