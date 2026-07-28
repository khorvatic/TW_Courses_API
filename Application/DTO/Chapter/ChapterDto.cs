using System;
using System.Collections.Generic;
using System.Text;

namespace Application.DTO.Chapter
{
    public class ChapterDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public TimeSpan Length { get; set; }
        public int CourseId { get; set; }
    }
}
