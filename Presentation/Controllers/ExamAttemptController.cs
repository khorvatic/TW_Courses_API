using Application.DTO.ExamAttempt;
using Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Presentation.Extensions;

namespace Presentation.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ExamAttemptController : ControllerBase
    {
        private readonly IExamAttemptService _examAttemptService;
        
        public ExamAttemptController(IExamAttemptService examAttemptService)
        {
            _examAttemptService = examAttemptService;
        }

        [Authorize]
        [HttpPost]
        public async Task<ActionResult<ExamAttemptDto>> CreateExamAttempt([FromBody] CreateExamAttemptDto dto)
        {
            var userId = User.GetUserId();

            if (userId != dto.UserId)
            {
                return Forbid();
            }

            var examAttempt = await _examAttemptService.CreateExamAttemptAsync(dto);
            return CreatedAtAction(nameof(GetExamAttemptById), new { id = examAttempt.Id }, examAttempt);
        }

        [Authorize]
        [HttpGet("{id}")]
        public async Task<ActionResult<ExamAttemptDto>> GetExamAttemptById(int id)
        {
            var examAttempt = await _examAttemptService.GetExamAttemptByIdAsync(id);
            var userId = User.GetUserId();

            if (userId != examAttempt.UserId && !User.IsInRole("Admin") && !User.IsInRole("Instructor"))
            {
                return Forbid("You don't have permission to access this exam attempt.");
            }

            return Ok(examAttempt);
        }

        [Authorize(Roles = "Admin")]
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ExamAttemptDto>>> GetAllExamAttempts()
        {
            var examAttempts = await _examAttemptService.GetAllExamAttemptsAsync();
            return Ok(examAttempts);
        }

        [Authorize(Roles = "Admin,Instructor")]
        [HttpGet("exam/{examId}")]
        public async Task<ActionResult<IEnumerable<ExamAttemptDto>>> GetExamAttemptsByExamId(int examId)
        {
            var examAttempts = await _examAttemptService.GetExamAttemptsByExamIdAsync(examId);
            return Ok(examAttempts);
        }

        [Authorize]
        [HttpGet("user/{userId}")]
        public async Task<ActionResult<IEnumerable<ExamAttemptDto>>> GetExamAttemptsByUserId(int userId)
        {
            var currUserId = User.GetUserId();
            
            if (currUserId != userId && !User.IsInRole("Admin") && !User.IsInRole("Instructor"))
            {
                return Forbid("You don't have permission to access this user's exam attempts.");
            }

            var examAttempts = await _examAttemptService.GetExamAttemptsByUserIdAsync(userId);
            return Ok(examAttempts);
        }

        [Authorize]
        [HttpPut("{attemptId}")]
        public async Task<ActionResult<ExamAttemptDto>> SubmitExam([FromRoute] int attemptId)
        {
            var examAttempt = await _examAttemptService.GetExamAttemptByIdAsync(attemptId);
            var userId = User.GetUserId();
            if (userId != examAttempt.UserId)
            {
                return Forbid();
            }

            var updatedExamAttempt = await _examAttemptService.SubmitExamAttemptAsync(attemptId);
            return Ok(updatedExamAttempt);
        }
    }
}
