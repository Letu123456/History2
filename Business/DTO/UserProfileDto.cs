using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business.DTO
{
    public class UserProfileDto
    {
        public string Email { get; set; }
        public string Usernmae { get; set; }
        public IList<string> roles { get; set; }
        public string Image {  get; set; }
        public string PhoneNumber {  get; set; }
        public string Address {  get; set; }
        public bool IsPremium {  get; set; }
    }
}
