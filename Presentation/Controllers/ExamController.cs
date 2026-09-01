using Application.DTO.Exam;
using Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Presentation.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ExamController : ControllerBase
    {
        private readonly IExamService _examService;

        public ExamController(IExamService examService)
        {
            _examService = examService;
        }

        [Authorize(Roles = "Admin,Instructor")]
        [HttpPost]
        public async Task<ActionResult<ExamDto>> CreateExam(CreateExamDto dto)
        {
            var exam = await _examService.CreateExamAsync(dto);
            return CreatedAtAction(nameof(GetExamById), new { id = exam.Id }, exam);
        }

        [Authorize]
        [HttpGet("{id}")]
        public async Task<ActionResult<ExamDto>> GetExamById([FromRoute] int id)
        {
            var exam = await _examService.GetExamByIdAsync(id);
            return Ok(exam);
        }

        [Authorize(Roles = "Admin,Instructor")]
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ExamDto>>> GetAllExams()
        {
            var exams = await _examService.GetAllExamsAsync();
            return Ok(exams);
        }

        [Authorize]
        [HttpGet("course/{courseId}")]
        public async Task<ActionResult<IEnumerable<ExamDto>>> GetExamsByCourseId([FromRoute] int courseId)
        {
            var exams = await _examService.GetExamsByCourseIdAsync(courseId);
            return Ok(exams);
        }

        [Authorize]
        [HttpGet("title")]
        public async Task<ActionResult<ExamDto>> GetExamByTitle([FromQuery] string title)
        {
            var exam = await _examService.GetExamByTitleAsync(title);
            return Ok(exam);
        }

        [Authorize(Roles = "Admin,Instructor")]
        [HttpPut("{id}")]
        public async Task<ActionResult<ExamDto>> UpdateExam([FromRoute] int id, [FromBody] CreateExamDto dto)
        {
            var exam = await _examService.UpdateExamAsync(id, dto);
            return Ok(exam);
        }

        [Authorize(Roles = "Admin,Instructor")]
        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteExam([FromRoute] int id)
        {
            await _examService.RemoveExamAsync(id);
            return Ok();
        }
    }
}
