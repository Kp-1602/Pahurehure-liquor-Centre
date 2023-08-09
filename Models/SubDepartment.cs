using System;
using System.Collections.Generic;

#nullable disable

namespace WebApplication8.Models
{
    public partial class SubDepartment
    {
        public SubDepartment()
        {
            Departments = new HashSet<Department>();
        }

        public int SubDepartmentId { get; set; }
        public string SubDepartmentName { get; set; }

        public virtual ICollection<Department> Departments { get; set; }
    }
}
