using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace WebApplication8.ViewModels
{
    public class OrderCustomerDetailsClass
    {
        [Key]
        public int OrderId { get; set; }
        public int Order_No { get; set; }
        public DateTime? Order_Date { get; set; }

        public int ProductId { get; set; }

        public string Name { get; set; }
        public string Image { get; set; }

        public int? Quantity { get; set; }
        public decimal? Price { get; set; }
        public decimal? Subtotal { get; set; }
        public string Customer_Email { get; set; }

        public string First_Name { get; set; }
        public string Last_Name { get; set; }
        public string Phone { get; set; }

    }
}
