using System;
using System.Collections.Generic;
using System.Text;

namespace Domain.Models
{
    public class EnrolledCourse
    {
        public int Id { get; set; }
        public bool Completed { get; set; }
        public int CourseId { get; set; }
        public Course Course { get; set; }
        public int UserId { get; set; }
        public User User { get; set; }
    }
}
