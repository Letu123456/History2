using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business.Model
{
    public class CategoryArtifact
    {
        [Key]
        public int Id { get; set; }
        public string Name { get; set; }
        public int? CategoryHistoricalId {  get; set; }
        [ForeignKey("CategoryHistoricalId")]
        public CategoryHistorical? CategoryHistorical { get; set; }
        public ICollection<Artifact>? Artifacts { get; set; } =new List<Artifact>();
    }
}
