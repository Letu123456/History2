using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business.Model
{
    public class Historical
    {
        [Key]
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Video { get; set; }
        public string Image { get; set; }
        public string Podcast { get; set; }
        public List<HistoricalImage> Images { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string TimeLine { get; set; }
        public string Location { get; set; }
        public string Gorvernance { get; set; }
        public string PolitialStructure { get; set; }
        public string Figure {  get; set; }
        public int? CategoryHistorycalId { get; set; }
        [ForeignKey("CategoryHistorycalId")]
        public CategoryHistorical? CategoryHistorical { get; set; }
        public ICollection<ArtifactHistorical>? ArtifactHistoricals { get; set; } = new List<ArtifactHistorical>();

        public ICollection<HistoricalFigure>? HistoricalFigures { get; set; } = new List<HistoricalFigure>();
    }
}
