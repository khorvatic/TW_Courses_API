using System;
using System.Collections.Generic;
using System.Text;

namespace Domain.Models
{
    public class Question
    {
        public int Id { get; set; }
        public string Text { get; set; }
        public QuestionType Type { get; set; }
        public int ExamId { get; set; }
        public Exam Exam { get; set; }
        public ICollection<Answer> Answers { get; set; }
        public ICollection<ExamQuestionAnswer> ExamQuestionAnswers { get; set; }
    }

    public enum QuestionType
    {
        MultipleChoice,
        TrueFalse
    }
}
