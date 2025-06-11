using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http; 
using Newtonsoft.Json; 

namespace Business.DTO
{
    public class HistoricalDto
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public IFormFile Video { get; set; }
        public IFormFile Image { get; set; }
        public IFormFile Podcast { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string TimeLine { get; set; }
        public string Location { get; set; }
        public string Gorvernance { get; set; }
        public string PolitialStructure { get; set; }
        public string Figure { get; set; }
        public int CategoryId {  get; set; }

        public int[] ArtifactIds { get; set; }
        public int[] FigureIds { get; set; }
    }
}
