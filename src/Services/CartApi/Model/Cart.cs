using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CartApi.Model
{
    public class Cart
    {
        public string BuyerId { get; set; }

        public List<CartItem> Items { get; set; }

        public Cart(string buyerId)
        {
            BuyerId = buyerId;
            Items = new List<CartItem>();
        }
    }
}
