using System;
using System.Collections.Generic;
using System.Text;

namespace Application.DTO.Answer
{
    public class AnswerDto
    {
        public int Id { get; set; }
        public int QuestionId { get; set; }
        public string Option { get; set; }
    }
}
