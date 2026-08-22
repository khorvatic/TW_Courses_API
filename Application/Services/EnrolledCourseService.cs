using Application.DTO.EnrolledCourse;
using Application.Interfaces;
using Domain.Exceptions;
using Domain.Interfaces;
using Domain.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Services
{
    public class EnrolledCourseService : IEnrolledCourseService
    {
        private readonly IUnitOfWork _unitOfWork;

        public EnrolledCourseService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<EnrolledCourseDto> EnrollInCourseAsync(CreateEnrolledCourseDto dto)
        {
            var enrolledCourses = await _unitOfWork.EnrolledCourses.GetByUserIdAndCourseIdAsync(dto.UserId, dto.CourseId);
            if (enrolledCourses.Any(ec => !ec.Completed))
            {
                throw new BusinessRuleException("User is already enrolled in this course and has not completed it yet.");
            }

            var enrolledCourse = new EnrolledCourse
            {
                Completed = false,
                CourseId = dto.CourseId,
                UserId = dto.UserId
            };

            await _unitOfWork.EnrolledCourses.AddAsync(enrolledCourse);
            await _unitOfWork.SaveChangesAsync();

            return new EnrolledCourseDto
            {
                Id = enrolledCourse.Id,
                Completed = enrolledCourse.Completed,
                CourseId = enrolledCourse.CourseId,
                UserId = enrolledCourse.UserId
            };
        }

        public async Task<EnrolledCourseDto> GetEnrolledCourseByIdAsync(int enrolledCourseId)
        {
            var enrolledCourse = await _unitOfWork.EnrolledCourses.GetByIdAsync(enrolledCourseId);
            if (enrolledCourse == null) throw new NotFoundException("Enrolled course with that ID not found");

            return new EnrolledCourseDto
            {
                Id = enrolledCourse.Id,
                Completed = enrolledCourse.Completed,
                CourseId = enrolledCourse.CourseId,
                UserId = enrolledCourse.UserId
            };
        }

        public async Task<IEnumerable<EnrolledCourseDto>> GetEnrolledCoursesForUserAsync(int userId)
        {
            var enrolledCourses = await _unitOfWork.EnrolledCourses.GetByUserIdAsync(userId);
            return enrolledCourses.Select(ec => new EnrolledCourseDto
            {
                Id = ec.Id,
                Completed = ec.Completed,
                CourseId = ec.CourseId,
                UserId = ec.UserId
            });
        }

        public async Task CompleteCourseAsync(int userId, int courseId)
        {
            var enrolledCourses = await _unitOfWork.EnrolledCourses.GetByUserIdAndCourseIdAsync(userId, courseId);
            var enrolledCourse = enrolledCourses.FirstOrDefault(ec => !ec.Completed);

            if (enrolledCourse == null) 
                throw new NotFoundException("User doesn't have any uncompleted Enrolled courses");

            enrolledCourse.Completed = true;
            _unitOfWork.EnrolledCourses.Update(enrolledCourse);
            await _unitOfWork.SaveChangesAsync();
        }
    }
}
