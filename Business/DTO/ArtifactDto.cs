using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business.DTO
{
    public class ArtifactDto
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public IFormFile Image {  get; set; }
        public IFormFile Podcast { get; set; }
        public string DateDiscovered { get; set; }
        public string Dimenson { get; set; }
        public string Weight { get; set; }
        public string Material { get; set; }
        public string Function { get; set; }
        public string Condition { get; set; }
        public string Origin { get; set; }
        //public Boolean Status { get; set; }

        public int? MuseumId { get; set; }
        public int? CategoryArtifactId { get; set; }
    }
}
