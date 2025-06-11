using Microsoft.EntityFrameworkCore.Storage.ValueConversion.Internal;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business.Model
{
    public class Event
    {
        [Key]
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        
        public DateTime StartDate { get; set; }
        
        public DateTime EndDate { get; set; }
        public string Image {  get; set; }
        public string Location {  get; set; }
        public List<Hastag> Hastag {  get; set; }
        public int? MuseumId { get; set; }
        [ForeignKey("MuseumId")]
        public Museum? Museum { get; set; }
        public ICollection<Comment>? Comments { get; set; } = new List<Comment>();
        public ICollection<RepliComment>? RepliComments { get; set; } = new List<RepliComment>();
    }
}
