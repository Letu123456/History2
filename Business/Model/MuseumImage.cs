using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business.Model
{
    public class MuseumImage
    {
        public int Id { get; set; }
        public string ImageUrl { get; set; }

        public int? MuseumId { get; set; }

        [ForeignKey("MuseumId")]
        public Museum Museum { get; set; }
    }
}
