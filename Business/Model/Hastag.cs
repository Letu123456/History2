using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business.Model
{
    public class Hastag
    {
        public int Id { get; set; }
        public string Hashtag { get; set; }

        public int? EventId { get; set; }

        [ForeignKey("EventId")]
        public Event? Event { get; set; }
    }
}
