using System;
using System.Collections.Generic;

#nullable disable

namespace WebApplication8.Models
{
    public partial class Card
    {
        public string Name { get; set; }
        public string stripeToken { get; set; }
        public string stripeEmail { get; set; }
        public long Amount { get; set; }
    }
}
