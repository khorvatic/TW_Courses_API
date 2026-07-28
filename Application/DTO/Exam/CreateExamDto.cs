using Application.DTO.Question;
using System;
using System.Collections.Generic;
using System.Text;

namespace Application.DTO.Exam
{
    public class CreateExamDto
    {
        public string Title { get; set; }
        public TimeSpan AllotedTime { get; set; }
        public int CourseId { get; set; }
        public ICollection<QuestionDto> Questions { get; set; }
    }
}
