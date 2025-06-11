using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business.Model
{
    public class ArtifactImage
    {
        public int Id { get; set; }
        public string ImageUrl {  get; set; }

        public int? Artifactid { get; set; }

        [ForeignKey("Artifactid")]
        public Artifact Artifact { get; set; }
    }
}
