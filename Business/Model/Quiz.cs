using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business.Model
{
    public class Quiz
    {
        [Key]
        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public int Level {  get; set; }
        public int TimeLimit { get; set; } 
        public bool IsActive { get; set; } 
        public DateTime CreatedAt { get; set; }
        public int? CategoryHistoricalId { get; set; }

        [ForeignKey("CategoryHistoricalId")]
        public CategoryHistorical? CategoryHistoricals { get; set; }

        public ICollection<Question>? Questions { get; set; }
        public ICollection<UserQuizResult>? QuizResults { get; set; } = new List<UserQuizResult>();
    }


}
