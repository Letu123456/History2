using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business.DTO
{
    public class GhnDistrict
    {
        //public int DistrictId { get; set; }
        //public string DistrictName { get; set; }

        [JsonProperty("DistrictID")]
        public int DistrictID { get; set; }

        [JsonProperty("DistrictName")]
        public string DistrictName { get; set; }

        [JsonProperty("ProvinceID")]
        public int ProvinceID { get; set; }

        [JsonProperty("Code")]
        public string Code { get; set; }
    }
}
