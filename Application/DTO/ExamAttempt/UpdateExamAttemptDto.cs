using System;
using System.Collections.Generic;
using System.Text;

namespace Application.DTO.ExamAttempt
{
    public class UpdateExamAttemptDto
    {
        public bool Passed { get; set; }
        public double Score { get; set; }
    }
}
