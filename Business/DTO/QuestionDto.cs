using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business.DTO
{
    public class QuestionDto
    {
        public int? QuizId { get; set; }
        public string Text { get; set; }
        public int Points { get; set; }
        public IFormFile Image { get; set; }
    }
}
