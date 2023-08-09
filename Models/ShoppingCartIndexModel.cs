using System;
using System.Collections.Generic;

namespace WebApplication8.Models
{
    public class ShoppingCartIndexModel
    {
        public List<ShoppingCartItem> ShoppingCart { get; set; }
        public Customer customer { get; set; }
        public Card card { get; set; }
        public decimal ShoppingCartTotal { get; set; }
        public DateTime? PickTime { get; set; }
        public string ReturnUrl { get; set; }
    }
}
