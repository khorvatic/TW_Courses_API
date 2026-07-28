using System;
using System.Collections.Generic;
using System.Text;

namespace Application.DTO.Course
{
    public class CreateCourseDto
    {
        public string Name { get; set; }
        public TimeSpan TimeToComplete { get; set; }

    }
}
