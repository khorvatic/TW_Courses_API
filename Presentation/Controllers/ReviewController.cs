using Application.DTO.Review;
using Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Presentation.Extensions;

namespace Presentation.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ReviewController : ControllerBase
    {
        private readonly IReviewService _reviewService;

        public ReviewController(IReviewService reviewService)
        {
            _reviewService = reviewService;
        }

        [Authorize]
        [HttpPost]
        public async Task<ActionResult<ReviewDto>> CreateReview([FromBody] CreateReviewDto createReviewDto)
        {
            var userId = User.GetUserId();
            var review = await _reviewService.CreateReviewAsync(createReviewDto, userId);

            return CreatedAtAction(nameof(GetReviewById), new { id = review.Id }, review);
        }

        [Authorize]
        [HttpGet("{id}")]
        public async Task<ActionResult<ReviewDto>> GetReviewById([FromRoute] int id)
        {
            var review = await _reviewService.GetReviewByIdAsync(id);
            return Ok(review);
        }

        [Authorize(Roles = "Admin")]
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ReviewDto>>> GetAllReviews()
        {
            var reviews = await _reviewService.GetAllReviewsAsync();
            return Ok(reviews);
        }

        [Authorize(Roles = "Admin")]
        [HttpGet("course/{courseId}")]
        public async Task<ActionResult<IEnumerable<ReviewDto>>> GetReviewsByCourseId([FromRoute] int courseId)
        {
            var reviews = await _reviewService.GetReviewsByCourseIdAsync(courseId);
            return Ok(reviews);
        }

        [Authorize(Roles = "Admin")]
        [HttpGet("user/{userId}")]
        public async Task<ActionResult<IEnumerable<ReviewDto>>> GetReviewsByUserId([FromRoute] int userId)
        {
            var reviews = await _reviewService.GetReviewsByUserIdAsync(userId);
            return Ok(reviews);
        }

        [Authorize]
        [HttpPut("{reviewId}")]
        public async Task<ActionResult<ReviewDto>> UpdateReview([FromRoute] int reviewId, [FromBody] UpdateReviewDto updateReviewDto)
        {
            var userId = User.GetUserId();
            var review = await _reviewService.GetReviewByIdAsync(reviewId);
            if (userId != review.UserId)
            {
                return Forbid("You are not authorized to update this review.");
            }

            var updatedReview = await _reviewService.UpdateReviewAsync(reviewId, updateReviewDto);
            return Ok(updatedReview);
        }

        [Authorize]
        [HttpDelete("{reviewId}")]
        public async Task<ActionResult> DeleteReview([FromRoute] int reviewId)
        {
            var userId = User.GetUserId();
            var review = await _reviewService.GetReviewByIdAsync(reviewId);
            if (userId != review.UserId && !User.IsInRole("Admin"))
            {
                return Forbid("You are not authorized to delete this review.");
            }
            
            await _reviewService.DeleteReviewAsync(reviewId);
            return Ok();
        }
    }
}
