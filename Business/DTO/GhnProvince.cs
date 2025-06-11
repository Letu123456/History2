using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business.DTO
{
    public class GhnProvince
    {
        //public int ProvinceId { get; set; }
        //public string ProvinceName { get; set; }
        [JsonProperty("ProvinceID")]
        public int ProvinceID { get; set; }

        [JsonProperty("ProvinceName")]
        public string ProvinceName { get; set; }

        [JsonProperty("Code")]
        public string Code { get; set; }
    }

   

}
