using System;
using System.Collections.Generic;
using System.Text;

namespace Domain.Models
{
    public class Chapter
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public TimeSpan Length { get; set; }
        public int CourseId { get; set; }
        public Course Course { get; set; }
    }
}
