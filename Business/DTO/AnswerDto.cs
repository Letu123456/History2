using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business.DTO
{
    public class AnswerDto
    {
        public int? QuestionId { get; set; }
        public string Text { get; set; }
        public bool IsCorrect { get; set; }
    }

}
