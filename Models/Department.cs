using System;
using System.Collections.Generic;

#nullable disable

namespace WebApplication8.Models
{
    public partial class Department
    {
        public Department()
        {
            Products = new HashSet<Product>();
        }

        public int DepartmentId { get; set; }
        public string Name { get; set; }
        public int SubDepartmentId { get; set; }

        public virtual SubDepartment SubDepartment { get; set; }
        public virtual ICollection<Product> Products { get; set; }
    }
}
