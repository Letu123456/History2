using Business.Model;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business.DTO
{
    public class FigureDto
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public string BirthDate { get; set; }
        public string DeathDate { get; set; }
        public string Era { get; set; }
        public string Occupation { get; set; }
        public IFormFile Image { get; set; }
        public IFormFile Podcast { get; set; }
        public int? CategoryFigureId {  get; set; }
        
    }
}
