using Application.DTO.Question;
using Application.Interfaces;
using Domain.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Presentation.Controllers
{
    public class QuestionController : ControllerBase
    {
        private readonly IQuestionService _questionService;

        public QuestionController(IQuestionService questionService)
        {
            _questionService = questionService;
        }

        [Authorize(Roles = "Admin,Instructor")]
        [HttpPost]
        public async Task<ActionResult<QuestionDto>> CreateQuestion(CreateQuestionDto dto)
        {
            var question = await _questionService.CreateQuestionAsync(dto);
            return CreatedAtAction(nameof(GetQuestionById), new { id = question.Id }, question);
        }

        [Authorize(Roles = "Admin")]
        [HttpGet("{id}")]
        public async Task<ActionResult<QuestionDto>> GetQuestionById([FromRoute] int id)
        {
            var question = await _questionService.GetQuestionByIdAsync(id);
            return Ok(question);
        }

        [Authorize(Roles = "Admin")]
        [HttpGet]
        public async Task<ActionResult<IEnumerable<QuestionDto>>> GetAllQuestions()
        {
            var questions = await _questionService.GetAllQuestionsAsync();
            return Ok(questions);
        }

        [Authorize(Roles = "Admin,Instructor")]
        [HttpGet("text")]
        public async Task<ActionResult<QuestionDto>> GetQuestionByText([FromQuery] string text)
        {
            var question = await _questionService.GetQuestionByTextAsync(text);
            return Ok(question);
        }

        [Authorize(Roles = "Admin,Instructor")]
        [HttpGet("exam/{examId}")]
        public async Task<ActionResult<IEnumerable<QuestionDto>>> GetQuestionsByExamId([FromRoute] int examId)
        {
            var questions = await _questionService.GetQuestionsByExamIdAsync(examId);
            return Ok(questions);
        }

        [Authorize(Roles = "Admin")]
        [HttpGet("type")]
        public async Task<ActionResult<IEnumerable<QuestionDto>>> GetQuestionsByType([FromQuery] QuestionType type)
        {
            var questions = await _questionService.GetQuestionsByTypeAsync(type);
            return Ok(questions);
        }

        [Authorize(Roles = "Admin,Instructor")]
        [HttpPut("{id}")]
        public async Task<ActionResult<QuestionDto>> UpdateQuestion([FromRoute] int id, [FromBody] CreateQuestionDto dto)
        {
            var question = await _questionService.UpdateQuestionAsync(id, dto);
            return Ok(question);
        }

        [Authorize(Roles = "Admin")]
        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteQuestion([FromRoute] int id)
        {
            await _questionService.DeleteQuestionAsync(id);
            return Ok();
        }
    }
}
