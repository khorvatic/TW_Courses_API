using Application.DTO.EnrolledCourse;
using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Interfaces
{
    public interface IEnrolledCourseService
    {
        Task<IEnumerable<EnrolledCourseDto>> GetEnrolledCoursesForUserAsync(int userId);
        Task<EnrolledCourseDto> GetEnrolledCourseByIdAsync(int enrolledCourseId);
        Task<EnrolledCourseDto> EnrollInCourseAsync(CreateEnrolledCourseDto dto);
        Task CompleteCourseAsync(int userId, int courseId);
    }
}
