using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business.DTO
{
    public class OrderListRequest
    {
        [JsonProperty("shop_id")]
        public int ShopId { get; set; }

        [JsonProperty("from_date")]
        public string FromDate { get; set; }

        [JsonProperty("to_date")]
        public string ToDate { get; set; }

        [JsonProperty("offset")]
        public int Offset { get; set; } = 0;

        [JsonProperty("limit")]
        public int Limit { get; set; } = 50;
    }

}
