using System;
using System.Collections.Generic;
using System.Text;

namespace Domain.Models
{
    public class ExamQuestionAnswer
    {
        public int AnswerId { get; set; }
        public int QuestionId { get; set; }
        public int AttemptId { get; set; }
        public Answer Answer { get; set; }
        public Question Question { get; set; }
        public ExamAttempt Attempt { get; set; }
    }
}
