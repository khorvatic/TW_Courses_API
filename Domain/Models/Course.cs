using System;
using System.Collections.Generic;
using System.Text;

namespace Domain.Models
{
    public class Course
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public TimeSpan TimeToComplete { get; set; }
        public ICollection<Chapter> Chapters { get; set; }
        public ICollection<EnrolledCourse> EnrolledCourses { get; set; }
    }
}
