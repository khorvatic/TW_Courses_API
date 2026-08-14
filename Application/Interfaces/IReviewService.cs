using Application.DTO.Review;
using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Interfaces
{
    public interface IReviewService
    {
        Task<IEnumerable<ReviewDto>> GetAllReviewsAsync();
        Task<ReviewDto> GetReviewByIdAsync(int id);
        Task<IEnumerable<ReviewDto>> GetReviewsByCourseIdAsync(int courseId);
        Task<IEnumerable<ReviewDto>> GetReviewsByUserIdAsync(int userId);
        Task<ReviewDto> CreateReviewAsync(CreateReviewDto createReviewDto, int userId);
        Task<ReviewDto> UpdateReviewAsync(int id, UpdateReviewDto updateReviewDto);
        Task DeleteReviewAsync(int id);
    }
}
