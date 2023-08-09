using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebApplication8.Models
{
    public class ShoppingCartItem
    {
        public int Id { get; set; }
        public Product product { get; set; }
        public int Quantity { get; set; }
        public string ShoppingCartId { get; set; }
    }
}
