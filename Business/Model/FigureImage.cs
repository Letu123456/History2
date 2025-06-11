using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business.Model
{
    public class FigureImage
    {
        public int Id { get; set; }
        public string ImageUrl { get; set; }

        public int? FigureId { get; set; }

        [ForeignKey("FigureId")]
        public Figure? Figure { get; set; }
    }
}
