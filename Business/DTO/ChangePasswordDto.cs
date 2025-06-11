using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business.DTO
{
    public class ChangePasswordDto
    {
        [PasswordPropertyText]
        public string CurrentPassword { get; set; }
        [PasswordPropertyText]

        public string NewPassword { get; set; }
    }
}
