using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business.Model
{
    public class HistoricalImage
    {
        public int Id { get; set; }
        public string ImageUrl { get; set; }

        public int? HistoricalId { get; set; }

        [ForeignKey("HistoricalId")]
        public Historical Historical { get; set; }
    }
}
