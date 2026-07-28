using System;
using System.Collections.Generic;
using System.Text;

namespace Application.DTO.Review
{
    public class CreateReviewDto
    {
        public string Text { get; set; }
        public int NumOfStars { get; set; }
        public int CourseId { get; set; }
    }
}
