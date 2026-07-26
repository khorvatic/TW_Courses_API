using System;
using System.Collections.Generic;
using System.Text;

namespace Domain.Models
{
    public class Review
    {
        public int Id { get; set; }
        public string Text { get; set; }
        public DateOnly DateOfReview { get; set; }
        public int NumOfStars { get; set; }
        public int CourseId { get; set; }
        public Course Course { get; set; }
        public int UserId { get; set; }
        public User User { get; set; }
    }
}
