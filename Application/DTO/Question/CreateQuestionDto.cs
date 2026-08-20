using Application.DTO.Answer;
using Domain.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace Application.DTO.Question
{
    public class CreateQuestionDto
    {
        public string Text { get; set; }
        public QuestionType Type { get; set; }
        public int ExamId { get; set; }
        public ICollection<CreateAnswerDto> Answers { get; set; }
    }
}
