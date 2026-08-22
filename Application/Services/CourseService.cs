using Application.DTO.Course;
using Application.Interfaces;
using Domain.Exceptions;
using Domain.Interfaces;
using Domain.Models;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;

namespace Application.Services
{
    public class CourseService : ICourseService
    {
        private readonly IUnitOfWork _unitOfWork;

        public CourseService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<CourseDto> CreateCourseAsync(CreateCourseDto createCourseDto)
        {
            var course = new Course
            {
                Name = createCourseDto.Name,
                TimeToComplete = createCourseDto.TimeToComplete
            };

            if (await _unitOfWork.Courses.GetCourseByNameAsync(course.Name) != null)
            {
                throw new BusinessRuleException("Course with the same name already exists");
            }

            await _unitOfWork.Courses.AddAsync(course);
            await _unitOfWork.SaveChangesAsync();

            return new CourseDto
            {
                Id = course.Id,
                Name = course.Name,
                TimeToComplete = course.TimeToComplete
            };
        }

        public async Task DeleteCourseAsync(int id)
        {
            var course = await _unitOfWork.Courses.GetByIdAsync(id);
            if (course == null) throw new NotFoundException("Cannot delete because Course with that ID not found");

            await _unitOfWork.Courses.DeleteAsync(id);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task<IEnumerable<CourseDto>> GetAllCoursesAsync()
        {
            var courses = await _unitOfWork.Courses.GetAllAsync();

            return courses.Select(course => new CourseDto
            {
                Id = course.Id,
                Name = course.Name,
                TimeToComplete = course.TimeToComplete
            });
        }

        public async Task<CourseDto> GetCourseByIdAsync(int id)
        {
            var course = await _unitOfWork.Courses.GetByIdAsync(id);
            if (course == null)
            {
                throw new NotFoundException("Course with that ID not found");
            }

            return new CourseDto
            {
                Id = course.Id,
                Name = course.Name,
                TimeToComplete = course.TimeToComplete
            };
        }

        public async Task<CourseDto> UpdateCourseAsync(int id, CreateCourseDto updateCourseDto)
        {
            var course = await _unitOfWork.Courses.GetByIdAsync(id);
            if (course == null) throw new NotFoundException("Cannot update because Course with that ID not found");

            course.Name = updateCourseDto.Name;
            course.TimeToComplete = updateCourseDto.TimeToComplete;
            
            _unitOfWork.Courses.Update(course);
            await _unitOfWork.SaveChangesAsync();
            
            return new CourseDto
            {
                Id = course.Id,
                Name = course.Name,
                TimeToComplete = course.TimeToComplete
            };
        }
    }
}
