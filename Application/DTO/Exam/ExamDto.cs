using Application.DTO.ExamAttempt;
using Application.DTO.Question;
using System;
using System.Collections.Generic;
using System.Text;

namespace Application.DTO.Exam
{
    public class ExamDto
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public TimeSpan AllotedTime { get; set; }
        public int CourseId { get; set; }
        public ICollection<QuestionDto> Questions { get; set; }
        public ICollection<ExamAttemptDto> ExamAttempts { get; set; }
    }
}
