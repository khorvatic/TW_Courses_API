using System;
using System.Collections.Generic;
using System.Text;

namespace Domain.Models
{
    public class ExamAttempt
    {
        public int Id { get; set; }
        public int ExamId { get; set; }
        public int UserId { get; set; }
        public bool Passed { get; set; }
        public double Score { get; set; }
        public Exam Exam { get; set; }
        public User User { get; set; }
        public ICollection<ExamQuestionAnswer> ExamQuestionAnswers { get; set; }
    }
}
