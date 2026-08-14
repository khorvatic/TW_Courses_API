using Application.DTO.Review;
using Application.Interfaces;
using Domain.Interfaces;
using Domain.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Services
{
    public class ReviewService : IReviewService
    {
        private readonly IUnitOfWork _unitOfWork;

        public ReviewService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<ReviewDto> CreateReviewAsync(CreateReviewDto createReviewDto, int userId)
        {
            var existingReviews = await _unitOfWork.Reviews.GetReviewsByCourseIdAsync(createReviewDto.CourseId);
            if (existingReviews.Any(r => r.UserId == userId))
                throw new InvalidOperationException("User has already submitted a review for this course.");

            var review = new Review
            {
                Text = createReviewDto.Text,
                DateOfReview = DateOnly.FromDateTime(DateTime.Now),
                NumOfStars = createReviewDto.NumOfStars,
                CourseId = createReviewDto.CourseId,
                UserId = userId
            };

            await _unitOfWork.Reviews.AddAsync(review);
            await _unitOfWork.SaveChangesAsync();

            return new ReviewDto
            {
                Id = review.Id,
                Text = review.Text,
                DateOfReview = review.DateOfReview,
                NumOfStars = review.NumOfStars,
                CourseId = review.CourseId,
                UserId = review.UserId
            };
        }

        public async Task DeleteReviewAsync(int id)
        {
            var review = await _unitOfWork.Reviews.GetByIdAsync(id);
            if (review == null)
                throw new ArgumentException("Review not found.");

            await _unitOfWork.Reviews.DeleteAsync(id);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task<IEnumerable<ReviewDto>> GetAllReviewsAsync()
        {
            var reviews = await _unitOfWork.Reviews.GetAllAsync();
            return reviews.Select(r => new ReviewDto
            {
                Id = r.Id,
                Text = r.Text,
                DateOfReview = r.DateOfReview,
                NumOfStars = r.NumOfStars,
                CourseId = r.CourseId,
                UserId = r.UserId
            });
        }

        public async Task<ReviewDto> GetReviewByIdAsync(int id)
        {
            var review = await _unitOfWork.Reviews.GetByIdAsync(id);
            if (review == null) throw new ArgumentException("Review not found.");

            return new ReviewDto
            {
                Id = review.Id,
                Text = review.Text,
                DateOfReview = review.DateOfReview,
                NumOfStars = review.NumOfStars,
                CourseId = review.CourseId,
                UserId = review.UserId
            };
        }

        public async Task<IEnumerable<ReviewDto>> GetReviewsByCourseIdAsync(int courseId)
        {
            var reviews = await _unitOfWork.Reviews.GetReviewsByCourseIdAsync(courseId);
            if (reviews == null || !reviews.Any()) throw new ArgumentException("No reviews found for this course.");

            return reviews.Select(r => new ReviewDto
            {
                Id = r.Id,
                Text = r.Text,
                DateOfReview = r.DateOfReview,
                NumOfStars = r.NumOfStars,
                CourseId = r.CourseId,
                UserId = r.UserId
            });
        }

        public async Task<IEnumerable<ReviewDto>> GetReviewsByUserIdAsync(int userId)
        {
            var reviews = await _unitOfWork.Reviews.GetReviewsByUserIdAsync(userId);
            if (reviews == null || !reviews.Any()) throw new ArgumentException("No reviews found for this user.");

            return reviews.Select(r => new ReviewDto
            {
                Id = r.Id,
                Text = r.Text,
                DateOfReview = r.DateOfReview,
                NumOfStars = r.NumOfStars,
                CourseId = r.CourseId,
                UserId = r.UserId
            });
        }

        public async Task<ReviewDto> UpdateReviewAsync(int id, UpdateReviewDto updateReviewDto)
        {
            var review = await _unitOfWork.Reviews.GetByIdAsync(id);
            if (review == null) throw new ArgumentException("Review not found.");

            review.Text = updateReviewDto.Text;
            review.NumOfStars = updateReviewDto.NumOfStars;

            _unitOfWork.Reviews.Update(review);
            await _unitOfWork.SaveChangesAsync();

            return new ReviewDto
            {
                Id = review.Id,
                Text = review.Text,
                DateOfReview = review.DateOfReview,
                NumOfStars = review.NumOfStars,
                CourseId = review.CourseId,
                UserId = review.UserId
            };
        }
    }
}
