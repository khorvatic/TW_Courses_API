using System;
using System.Collections.Generic;
using System.Text;

namespace Domain.Models
{
    public class Exam
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public TimeSpan AllotedTime { get; set; }
        public int CourseId { get; set; }
        public Course Course { get; set; }
        public ICollection<Question> Questions { get; set; }
        public ICollection<ExamAttempt> Attempts { get; set; }

    }
}
