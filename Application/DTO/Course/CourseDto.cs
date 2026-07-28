using Application.DTO.Chapter;
using System;
using System.Collections.Generic;
using System.Text;

namespace Application.DTO.Course
{
    public class CourseDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public TimeSpan TimeToComplete { get; set; }
        public IEnumerable<ChapterDto> Chapters { get; set; }
    }
}
