using Application.DTO.ExamQuestionAnswer;
using Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Presentation.Extensions;

namespace Presentation.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ExamQuestionAnswerController : ControllerBase
    {
        private readonly IExamQuestionAnswerService _examQuestionAnswerService;
        private readonly IExamAttemptService _examAttemptService;

        public ExamQuestionAnswerController(IExamQuestionAnswerService examQuestionAnswerService, IExamAttemptService examAttemptService)
        {
            _examQuestionAnswerService = examQuestionAnswerService;
            _examAttemptService = examAttemptService;
        }

        [Authorize]
        [HttpPost]
        public async Task<ActionResult<ExamQuestionAnswerDto>> CreateExamQuestionAnswer(CreateExamQuestionAnswerDto dto)
        {
            var attempt = await _examAttemptService.GetExamAttemptByIdAsync(dto.AttemptId);
            var userId = User.GetUserId();
            if (attempt.UserId != userId)
            {
                return Forbid("You can't create an answer for an attempt that doesn't belong to you.");
            }

            var eqa = await _examQuestionAnswerService.CreateExamQuestionAnswerAsync(dto);
            return CreatedAtAction(nameof(GetExamQuestionAnswerById), 
                new { questionId = eqa.QuestionId, answerId = eqa.AnswerId, attemptId = eqa.AttemptId },
                eqa);
        }

        [Authorize(Roles = "Admin,Instructor")]
        [HttpGet("{questionId}/{answerId}/{attemptId}")]
        public async Task<ActionResult<ExamQuestionAnswerDto>> GetExamQuestionAnswerById(
            [FromRoute] int questionId, [FromRoute] int answerId, [FromRoute] int attemptId)
        {
            var examQuestionAnswer = await _examQuestionAnswerService.GetExamQuestionAnswerByIdAsync(questionId, answerId, attemptId);
            return Ok(examQuestionAnswer);
        }

        [Authorize(Roles = "Admin")]
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ExamQuestionAnswerDto>>> GetAllExamQuestionAnswers()
        {
            var examQuestionAnswers = await _examQuestionAnswerService.GetAllExamQuestionAnswersAsync();
            return Ok(examQuestionAnswers);
        }

        [Authorize(Roles = "Admin,Instructor")]
        [HttpGet("by-answer/{answerId}")]
        public async Task<ActionResult<IEnumerable<ExamQuestionAnswerDto>>> GetExamQuestionAnswersByAnswerId([FromRoute] int answerId)
        {
            var examQuestionAnswers = await _examQuestionAnswerService.GetExamQuestionAnswerByAnswerIdAsync(answerId);
            return Ok(examQuestionAnswers);
        }

        [Authorize(Roles = "Admin,Instructor")]
        [HttpGet("by-question/{questionId}")]
        public async Task<ActionResult<IEnumerable<ExamQuestionAnswerDto>>> GetExamQuestionAnswersByQuestionId([FromRoute] int questionId)
        {
            var examQuestionAnswers = await _examQuestionAnswerService.GetExamQuestionAnswerByQuestionIdAsync(questionId);
            return Ok(examQuestionAnswers);
        }

        [Authorize(Roles = "Admin,Instructor")]
        [HttpGet("by-attempt/{attemptId}")]
        public async Task<ActionResult<IEnumerable<ExamQuestionAnswerDto>>> GetExamQuestionAnswersByAttemptId([FromRoute] int attemptId)
        {
            var examQuestionAnswers = await _examQuestionAnswerService.GetExamQuestionAnswerByAttemptIdAsync(attemptId);
            return Ok(examQuestionAnswers);
        }

        [Authorize(Roles = "Admin")]
        [HttpDelete("{questionId}/{answerId}/{attemptId}")]
        public async Task<ActionResult> DeleteExamQuestionAnswer(
            [FromRoute] int questionId, [FromRoute] int answerId, [FromRoute] int attemptId)
        {
            await _examQuestionAnswerService.DeleteExamQuestionAnswerAsync(answerId, questionId, attemptId);
            return Ok();
        }
    }
}
