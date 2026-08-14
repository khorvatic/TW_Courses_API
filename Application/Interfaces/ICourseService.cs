using Application.DTO.Course;
using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Interfaces
{
    public interface ICourseService
    {
        Task<IEnumerable<CourseDto>> GetAllCoursesAsync();
        Task<CourseDto> GetCourseByIdAsync(int id);
        Task<CourseDto> CreateCourseAsync(CreateCourseDto createCourseDto);
        Task<CourseDto> UpdateCourseAsync(int id, CreateCourseDto updateCourseDto);
        Task DeleteCourseAsync(int id);
    }
}
