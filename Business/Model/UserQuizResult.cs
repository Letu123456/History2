using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business.Model
{
    public class UserQuizResult
    {
        [Key]
        public int Id { get; set; }
        public string? UserId { get; set; }

        [ForeignKey("UserId")]
        public User? user { get; set; }
        public int? QuizId { get; set; }

        [ForeignKey("QuizId")]
        public Quiz? Quiz { get; set; }
        public int Score { get; set; }
        public DateTime CompletedAt { get; set; }

    }

}
