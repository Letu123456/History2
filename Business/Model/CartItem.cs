﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business.Model
{
    public class CartItem
    {
        public int Id { get; set; }
        public int? CartId {  get; set; }
        [ForeignKey("CartId")]
        public Cart? Cart { get; set; }
        public int? ProductId {  get; set; }
        [ForeignKey("ProductId")]
        public Product? Product { get; set; }
        public int Quatity {  get; set; }
    }
}
