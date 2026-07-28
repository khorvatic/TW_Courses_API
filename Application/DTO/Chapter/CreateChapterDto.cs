using System;
using System.Collections.Generic;
using System.Text;

namespace Application.DTO.Chapter
{
    public class CreateChapterDto
    {
        public string Name { get; set; }
        public TimeSpan Length { get; set; }
    }
}
