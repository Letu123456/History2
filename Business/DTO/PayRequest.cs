using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Business.DTO
{

    

    public class PayRequest
    {

        [JsonProperty("orderCode")]
        public string orderCode { get; set; }

        [JsonProperty("amount")]
        public int amount { get; set; }

        [JsonProperty("description")]
        public string description { get; set; }

        [JsonProperty("returnUrl")]
        public string returnUrl { get; set; }

        [JsonProperty("cancelUrl")]
        public string cancelUrl { get; set; }

        [JsonProperty("signature")]
        public string signature { get; set; }
    }


}
