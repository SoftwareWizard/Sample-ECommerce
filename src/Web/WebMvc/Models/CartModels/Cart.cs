﻿using System;
using System.Collections.Generic;
using System.Linq;

namespace ShoesOnContainers.Web.WebMvc.Models.CartModels
{
    public class Cart
    {
        public List<CartItem> Items { get; set; } = new List<CartItem>();
        public string BuyerId { get; set; }

        public decimal Total()
        {
            return Math.Round(Items.Sum(x => x.UnitPrice * x.Quantity), 2);
        }
    }
}