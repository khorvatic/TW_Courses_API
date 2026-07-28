using Application.DTO.Answer;
using Domain.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace Application.DTO.Question
{
    public class CreateQuestionDto
    {
        public QuestionType Type { get; set; }
        public int ExamId { get; set; }
        public ICollection<AnswerDto> Answers { get; set; }
    }
}
