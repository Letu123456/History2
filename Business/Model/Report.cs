using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business.Model
{
    public class Report
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Content { get; set; }
        public Boolean? Status {  get; set; }
        public DateTime Created { get; set; } = DateTime.Now;

        public string? UserId {  get; set; }
        [ForeignKey("UserId")]
        public User? User { get; set; }


    }
}
