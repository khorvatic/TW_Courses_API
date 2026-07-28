using System;
using System.Collections.Generic;
using System.Text;

namespace Application.DTO.ExamQuestionAnswer
{
    public class ExamQuestionAnswerDto
    {
        public int AnswerId { get; set; }
        public int QuestionId { get; set; }
        public int AttemptId { get; set; }
    }
}
