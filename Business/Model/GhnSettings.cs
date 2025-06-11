using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business.Model
{
    public class GhnSettings
    {
        public string Token { get; set; }
        public string ShopId { get; set; }
        public int DefaultFromDistrictId { get; set; }  // E.g., 1454
        public string DefaultFromWardCode { get; set; }  // E.g., "21211"
       
    }
}
