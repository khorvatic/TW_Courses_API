using Application.DTO.EnrolledCourse;
using Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.JSInterop.Infrastructure;
using Presentation.Extensions;

namespace Presentation.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class EnrolledCourseController : ControllerBase
    {
        private readonly IEnrolledCourseService _enrolledCourseService;

        public EnrolledCourseController(IEnrolledCourseService enrolledCourseService)
        {
            _enrolledCourseService = enrolledCourseService;
        }

        [Authorize]
        [HttpPost]
        public async Task<ActionResult<EnrolledCourseDto>> EnrollInCourse([FromBody] CreateEnrolledCourseDto dto)
        {
            var userId = User.GetUserId();
            if (userId == dto.UserId)
            {
                var enrolledCourse = await _enrolledCourseService.EnrollInCourseAsync(dto);
                return Ok(enrolledCourse); 
            }
            return BadRequest("User ID-s do not match");
        }

        [Authorize]
        [HttpGet("{enrolledCourseId}")]
        public async Task<ActionResult<EnrolledCourseDto>> GetEnrolledCourseById([FromRoute] int enrolledCourseId)
        {
            var userId = User.GetUserId();
            var enrolledCourse = await _enrolledCourseService.GetEnrolledCourseByIdAsync(enrolledCourseId);
            if (userId == enrolledCourse.UserId && User.IsInRole("Admin"))
            {
                return Ok(enrolledCourse); 
            }
            return Forbid("You are not authorized to access this enrolled course.");
        }

        [Authorize]
        [HttpGet("user/{userId}")]
        public async Task<ActionResult<IEnumerable<EnrolledCourseDto>>> GetEnrolledCoursesByUserId([FromRoute] int userId)
        {
            var currUserId = User.GetUserId();
            if (currUserId != userId && !User.IsInRole("Admin"))
            {
                return Forbid("You are not authorized to access this user's enrolled courses.");
            }

            var enrolledCourses = await _enrolledCourseService.GetEnrolledCoursesForUserAsync(userId);
            return Ok(enrolledCourses);
        }

        [Authorize]
        [HttpPut("complete")]
        public async Task<ActionResult<EnrolledCourseDto>> CompleteEnrolledCourse([FromQuery] int userId, [FromQuery] int courseId)
        {
            var currUserId = User.GetUserId();
            if (currUserId != userId && !User.IsInRole("Admin"))
            {
                return Forbid("You are not authorized to complete this course.");
            }

            await _enrolledCourseService.CompleteCourseAsync(userId, courseId);
            return Ok();
        }
    }
}
