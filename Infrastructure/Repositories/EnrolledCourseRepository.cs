using Domain.Interfaces;
using Domain.Models;
using Infrastructure.Data;
using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories
{
    public class EnrolledCourseRepository : IEnrolledCourseRepository
    {
        private readonly CourseContext _courseContext;
        public EnrolledCourseRepository(CourseContext courseContext)
        {
            _courseContext = courseContext;
        }

        public async Task AddAsync(EnrolledCourse entity)
        {
            await _courseContext.EnrolledCourses.AddAsync(entity);
        }

        public async Task DeleteAsync(int id)
        {
            var enrolledCourse = await _courseContext.EnrolledCourses.FindAsync(id);
            if (enrolledCourse != null)
            {
                _courseContext.EnrolledCourses.Remove(enrolledCourse);
            }
        }

        public async Task<IEnumerable<EnrolledCourse>> GetAllAsync()
        {
            return await _courseContext.EnrolledCourses.ToListAsync();
        }

        public async Task<IEnumerable<EnrolledCourse>> GetByCourseIdAsync(int courseId)
        {
            return await _courseContext.EnrolledCourses
                .Where(ec => ec.CourseId == courseId)
                .ToListAsync();
        }

        public async Task<EnrolledCourse> GetByIdAsync(int id)
        {
            return await _courseContext.EnrolledCourses.FindAsync(id);
        }

        public async Task<IEnumerable<EnrolledCourse>> GetByUserIdAndCourseIdAsync(int userId, int courseId)
        {
            return await _courseContext.EnrolledCourses
                .Where(ec => ec.UserId == userId && ec.CourseId == courseId)
                .ToListAsync();
        }

        public async Task<IEnumerable<EnrolledCourse>> GetByUserIdAsync(int userId)
        {
            return await _courseContext.EnrolledCourses
                .Where(ec => ec.UserId == userId)
                .ToListAsync();
        }

        public void Update(EnrolledCourse entity)
        {
            _courseContext.EnrolledCourses.Update(entity);
        }
    }
}
