using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business.Model
{
    public class Question
    {
        [Key]
        public int Id { get; set; }
        public int? QuizId { get; set; }

        [ForeignKey("QuizId")]
        public Quiz? Quiz { get; set; }
        public string Text { get; set; }
        public int Points { get; set; }
        public string? Image {  get; set; }

        public ICollection<Answer>? Answers { get; set; }
    }

}
