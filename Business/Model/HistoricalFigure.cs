using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business.Model
{
    public class HistoricalFigure
    {
        [Key]
        public int Id { get; set; }
        public int? FigureId { get; set; }
        public int? HistoricalId { get; set; }
        [ForeignKey("FigureId")]
        public Figure? Figure { get; set; }
        [ForeignKey("HistoricalId")]
        public Historical? Historical { get; set; }
    }
}
