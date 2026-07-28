using System;
using System.Collections.Generic;
using System.Text;

namespace Application.DTO.Review
{
    public class ReviewDto
    {
        public int Id { get; set; }
        public string Text { get; set; }
        public DateOnly DateOfReview { get; set; }
        public int NumOfStars { get; set; }
        public int CourseId { get; set; }
        public int UserId { get; set; }
    }
}
