using Domain.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace Domain.Interfaces
{
    public interface IReviewRepository : IGenericRepository<Review>
    {
        Task<IEnumerable<Review>> GetReviewsByCourseIdAsync(int courseId);
        Task<IEnumerable<Review>> GetReviewsByUserIdAsync(int userId);
    }
}
