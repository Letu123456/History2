﻿using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business.DTO
{
    public class GhnWard
    {
        [JsonProperty("WardCode")]
        public string WardCode { get; set; }

        [JsonProperty("WardName")]
        public string WardName { get; set; }

        [JsonProperty("DistrictID")]
        public int DistrictID { get; set; }
    }
}
