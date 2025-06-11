using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business.DTO
{
    public class MuseumDto
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public string Location { get; set; }
        public string EstablishYear { get; set; }
        public string Contact { get; set; }
        public IFormFile Image { get; set; }
        public IFormFile Video { get; set; }
    }
}
