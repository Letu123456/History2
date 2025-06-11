using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business.Model
{
    public class Favorite
    {
        public int Id { get; set; }
        public Boolean isSaveButton { get; set; }

        public string? UserId { get; set; }
        public int? ArtifactId { get; set; }
        [ForeignKey("UserId")]
        public User? User {  get; set; }
        [ForeignKey("ArtifactId")]
        public Artifact? Artifact { get; set; }

    }
}
