using System;
using System.Collections.Generic;

#nullable disable

namespace WebApplication8.Models
{
    public partial class Product
    {
        public Product()
        {
            OrderDetails = new HashSet<OrderDetail>();
        }

        public int ProductId { get; set; }
        public string Name { get; set; }
        public decimal? Price { get; set; }
        public string Description { get; set; }
        public string Image { get; set; }
        public int? DepartmentId { get; set; }
        public int? Stock { get; set; }

        public virtual Department Department { get; set; }
        public virtual ICollection<OrderDetail> OrderDetails { get; set; }
    }
}
