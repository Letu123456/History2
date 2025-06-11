using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business.Model
{
    public class Artifact
    {
        [Key]
        public int Id { get; set; }
        public string ArtifactName { get; set; }
        public string Description { get; set;}
        public string Image {  get; set;}
        public string Podcast {  get; set;}
        public List<ArtifactImage> Images { get; set; }
        public string DateDiscovered { get; set; }
        public string Dimenson { get; set; }
        public string Weight {  get; set; }
        public string Material {  get; set; }
        public string Function {  get; set; }
        public string Condition { get; set; }
        public string Origin { get; set; }
        public Boolean Status { get; set; }

        public int? CategoryArtifactId { get; set; }
        [ForeignKey("CategoryArtifactId")]
        public CategoryArtifact? CategoryArtifacts { get; set; }

        public int? MuseumId { get; set; }
        [ForeignKey("MuseumId")]
        public Museum? Museum { get; set; }
        public ICollection<ArtifactHistorical>? ArtifactHistoricals { get; set; } = new List<ArtifactHistorical>();
        
        public ICollection<Favorite>? SaveButtons { get; set; } = new List<Favorite>();

    }
}
