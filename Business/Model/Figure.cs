using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business.Model
{
    public class Figure
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string BirthDate { get; set; }
        public string DeathDate {  get; set; }
        public string Era { get; set; }
        public string Occupation {  get; set; }
        public string Image { get; set; }
        public string Podcast { get; set; }
        public List<FigureImage> Images { get; set; }
        public int? CategoryFigureId { get; set; }
        [ForeignKey("CategoryFigureId")]
        public CategoryFigure? CategoryFigure{ get; set; }
        public ICollection<HistoricalFigure>? HistoricalFigures { get; set; } = new List<HistoricalFigure>();
    }
}
