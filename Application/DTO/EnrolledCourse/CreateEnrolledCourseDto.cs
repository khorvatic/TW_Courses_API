using System;
using System.Collections.Generic;
using System.Text;

namespace Application.DTO.EnrolledCourse
{
    public class CreateEnrolledCourseDto
    {
        public bool Completed { get; set; }
        public int CourseId { get; set; }
        public int UserId { get; set; }
    }
}
