using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business.Model
{
    public class ArtifactHistorical
    {
        [Key]
        public int Id {  get; set; }
        public int? ArtifacrId { get; set; }
        public int? HistoricalId { get; set; }
        [ForeignKey("ArtifacrId")]
        public Artifact? Artifact { get; set; }
        [ForeignKey("HistoricalId")]
        public Historical? Historical { get; set; }
    }
}
