using System;
using System.Collections.Generic;

#nullable disable

namespace WebApplication8.Models
{
    public partial class Order
    {
        public Order()
        {
            OrderDetails = new HashSet<OrderDetail>();
        }

        public int OrderId { get; set; }
        public int OrderNo { get; set; }
        public DateTime? OrderDate { get; set; }
        public decimal? OrderTotal { get; set; }
        public int? CustomerId { get; set; }
        public string IsPaid { get; set; }
        public string IsDelivered { get; set; }
        public DateTime? PickUpTime { get; set; }
        public string PaymentType { get; set; }

        public virtual Customer Customer { get; set; }
        public virtual ICollection<OrderDetail> OrderDetails { get; set; }
    }
}
