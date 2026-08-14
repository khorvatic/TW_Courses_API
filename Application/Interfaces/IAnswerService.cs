using Application.DTO.Answer;
using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Interfaces
{
    public interface IAnswerService
    {
        Task<AnswerDto> CreateAnswerAsync(CreateAnswerDto createAnswerDto);
        Task<AnswerDto> UpdateAnswerAsync(int id, CreateAnswerDto updateAnswerDto);
        Task DeleteAnswerAsync(int id);
        Task<IEnumerable<AnswerDto>> GetAllAnswersAsync();
        Task<AnswerDto> GetAnswerByIdAsync(int id);
    }
}
