using System;
using System.Collections.Generic;
using System.Text;

namespace Application.DTO.ExamAttempt
{
    public class ExamAttemptDto
    {
        public int Id { get; set; }
        public int ExamId { get; set; }
        public int UserId { get; set; }
        public bool Passed { get; set; }
        public double Score { get; set; }
    }
}
