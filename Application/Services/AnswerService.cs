using Application.DTO.Answer;
using Application.Interfaces;
using Domain.Exceptions;
using Domain.Interfaces;
using Domain.Models;
using System;
using System.Collections.Generic;
using System.Net.WebSockets;
using System.Text;

namespace Application.Services
{
    public class AnswerService : IAnswerService
    {
        private readonly IUnitOfWork _unitOfWork;

        public AnswerService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<AnswerDto> CreateAnswerAsync(CreateAnswerDto createAnswerDto)
        {
            var answer = new Answer
            {
                QuestionId = createAnswerDto.QuestionId,
                Option = createAnswerDto.Option
            };

            await _unitOfWork.Answers.AddAsync(answer);
            await _unitOfWork.SaveChangesAsync();
            
            return new AnswerDto
            {
                Id = answer.Id,
                QuestionId = answer.QuestionId,
                Option = answer.Option
            };
        }

        public async Task DeleteAnswerAsync(int id)
        {
            var answer = await _unitOfWork.Answers.GetByIdAsync(id);
            if (answer == null) throw new NotFoundException("Can't delete because Answer with that ID not found");

            await _unitOfWork.Answers.DeleteAsync(id);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task<IEnumerable<AnswerDto>> GetAllAnswersAsync()
        {
            var answers = await _unitOfWork.Answers.GetAllAsync();
            
            return answers.Select(a => new AnswerDto
            {
                Id = a.Id,
                QuestionId = a.QuestionId,
                Option = a.Option
            });
        }

        public async Task<AnswerDto> GetAnswerByIdAsync(int id)
        {
            var answer = await _unitOfWork.Answers.GetByIdAsync(id);
            if ( answer == null ) throw new NotFoundException("Answer with that ID not found");

            return new AnswerDto
            {
                Id = answer.Id,
                QuestionId = answer.QuestionId,
                Option = answer.Option
            };
        }

        public async Task<AnswerDto> UpdateAnswerAsync(int id, CreateAnswerDto updateAnswerDto)
        {
            var answer = await _unitOfWork.Answers.GetByIdAsync(id);
            if (answer == null) throw new NotFoundException("Unable to update Answer with that ID because it was not found");

            answer.QuestionId = updateAnswerDto.QuestionId;
            answer.Option = updateAnswerDto.Option;

            _unitOfWork.Answers.Update(answer);
            await _unitOfWork.SaveChangesAsync();

            return new AnswerDto
            {
                Id = answer.Id,
                QuestionId = answer.QuestionId,
                Option = answer.Option
            };
        }
    }
}
