using System;
using System.Collections.Generic;
using System.Text;

namespace Domain.Models
{
    public class Answer
    {
        public int Id { get; set; }
        public int QuestionId { get; set; }
        public Question Question { get; set; }
        public string Option { get; set; }
        public bool Correct { get; set; }
        public ICollection<ExamQuestionAnswer> ExamQuestionAnswers { get; set; }
    }
}
