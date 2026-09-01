using Application.DTO.Answer;
using Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Presentation.Controllers
{
    public class AnswerController : ControllerBase
    {
        private readonly IAnswerService _answerService;

        public AnswerController(IAnswerService answerService)
        {
            _answerService = answerService;
        }

        [Authorize(Roles = "Admin,Instructor")]
        [HttpPost]
        public async Task<ActionResult<AnswerDto>> CreateAnswer(CreateAnswerDto dto)
        {
            var answer = await _answerService.CreateAnswerAsync(dto);
            return CreatedAtAction(nameof(GetAnswerById), new { id = answer.Id }, answer);
        }

        [Authorize(Roles = "Admin")]
        [HttpGet("{id}")]
        public async Task<ActionResult<AnswerDto>> GetAnswerById(int id)
        {
            var answer = await _answerService.GetAnswerByIdAsync(id);
            return Ok(answer);
        }

        [Authorize(Roles = "Admin")]
        [HttpGet]
        public async Task<ActionResult<IEnumerable<AnswerDto>>> GetAllAnswers()
        {
            var answers = await _answerService.GetAllAnswersAsync();
            return Ok(answers);
        }

        [Authorize(Roles = "Admin,Instructor")]
        [HttpPut("{id}")]
        public async Task<ActionResult<AnswerDto>> UpdateAnswer([FromRoute] int id, [FromBody] CreateAnswerDto dto)
        {
            var answer = await _answerService.UpdateAnswerAsync(id, dto);
            return Ok(answer);
        }

        [Authorize(Roles = "Admin")]
        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteAnswer([FromRoute] int id)
        {
            await _answerService.DeleteAnswerAsync(id);
            return Ok();
        }
    }
}
