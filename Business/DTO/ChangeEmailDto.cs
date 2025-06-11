using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business.DTO
{
    public class ChangeEmailDto
    {
        [EmailAddress]
        public string newEmail { get; set; }
    }
}
