using System;
using System.Collections.Generic;
using System.Text;

namespace Application.DTO.Answer
{
    public class CreateAnswerDto
    {
        public int QuestionId { get; set; }
        public string Option { get; set; }
        public bool Correct { get; set; }
    }
}
