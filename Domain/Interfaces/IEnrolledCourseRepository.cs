using Domain.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace Domain.Interfaces
{
    public interface IEnrolledCourseRepository : IGenericRepository<EnrolledCourse>
    {
        Task<IEnumerable<EnrolledCourse>> GetByUserIdAsync(int userId);
        Task<IEnumerable<EnrolledCourse>> GetByCourseIdAsync(int courseId);
        Task<IEnumerable<EnrolledCourse>> GetByUserIdAndCourseIdAsync(int userId, int courseId);
    }
}
