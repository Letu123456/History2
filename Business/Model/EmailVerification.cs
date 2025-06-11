using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business.Model
{
    public class EmailVerification
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public string id { get; set; }
        public string token { get; set; }

        public User User { get; set; }
        public string UserId { get; set; }

        public DateTime ExpirationDate { get; set; }
    }
}
