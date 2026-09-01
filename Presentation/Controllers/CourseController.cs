using Application.DTO.Course;
using Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Presentation.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CourseController : ControllerBase
    {
        private readonly ICourseService _courseService;

        public CourseController(ICourseService courseService)
        {
            _courseService = courseService;
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<ActionResult<CourseDto>> CreateCourse(CreateCourseDto dto)
        {
            var course = await _courseService.CreateCourseAsync(dto);

            return CreatedAtAction(nameof(GetCourseById), new { id = course.Id }, course);
        }

        [Authorize]
        [HttpGet("{id}")]
        public async Task<ActionResult<CourseDto>> GetCourseById([FromRoute] int id)
        {
            var course = await _courseService.GetCourseByIdAsync(id);
            return Ok(course);
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<CourseDto>>> GetAllCourses()
        {
            var courses = await _courseService.GetAllCoursesAsync();
            return Ok(courses);
        }

        [HttpGet("by-name")]
        public async Task<ActionResult<CourseDto>> GetCourseByName([FromQuery] string name)
        {
            var course = await _courseService.GetCourseByNameAsync(name);
            return Ok(course);
        }

        [Authorize(Roles = "Admin")]
        [HttpPut("{id}")]
        public async Task<ActionResult<CourseDto>> UpdateCourse([FromRoute] int id, CreateCourseDto dto)
        {
            return Ok(await _courseService.UpdateCourseAsync(id, dto));
        }

        [Authorize(Roles = "Admin")]
        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteCourse([FromRoute] int id) 
        { 
            await _courseService.DeleteCourseAsync(id);
            return Ok();
        }
    }
}
