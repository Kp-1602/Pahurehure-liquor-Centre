using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using WebApplication8.Models;
using WebApplication8.ViewModels;

#nullable disable

namespace WebApplication8.Models
{
    public partial class liquorstoredbContext : DbContext
    {
        public liquorstoredbContext()
        {
        }

        public liquorstoredbContext(DbContextOptions<liquorstoredbContext> options)
            : base(options)
        {
        }

        public virtual DbSet<Customer> Customers { get; set; }
        public virtual DbSet<Department> Departments { get; set; }
        public virtual DbSet<Order> Orders { get; set; }
        public virtual DbSet<OrderDetail> OrderDetails { get; set; }
        public virtual DbSet<Product> Products { get; set; }
        public virtual DbSet<SubDepartment> SubDepartments { get; set; }
        
        public virtual DbSet<OrderCustomerDetailsClass> OrderCustomerDetailsClasses { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see http://go.microsoft.com/fwlink/?LinkId=723263.
                optionsBuilder.UseSqlServer("Server=liquorstore.database.windows.net;Database=liquorstoredb;UID=liquoradmin;PWD=Password1@;encrypt=true;trustservercertificate=true");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.HasAnnotation("Relational:Collation", "SQL_Latin1_General_CP1_CI_AS");

            modelBuilder.Entity<Customer>(entity =>
            {
                entity.ToTable("Customer");

                entity.Property(e => e.CustomerId).HasColumnName("Customer_Id");

                entity.Property(e => e.City).HasMaxLength(50);

                entity.Property(e => e.CustomerEmail)
                    .HasMaxLength(50)
                    .HasColumnName("Customer_Email");

                entity.Property(e => e.FirstName)
                    .HasMaxLength(50)
                    .HasColumnName("First_Name");

                entity.Property(e => e.LastName)
                    .HasMaxLength(50)
                    .HasColumnName("Last_Name");

                entity.Property(e => e.Phone).HasMaxLength(50);
            });

            modelBuilder.Entity<Department>(entity =>
            {
                entity.ToTable("Department");

                entity.Property(e => e.Name)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.SubDepartmentId).HasColumnName("Sub_DepartmentId");

                entity.HasOne(d => d.SubDepartment)
                    .WithMany(p => p.Departments)
                    .HasForeignKey(d => d.SubDepartmentId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Department_Sub_Department");
            });

            modelBuilder.Entity<Order>(entity =>
            {
                entity.ToTable("Order");

                entity.Property(e => e.OrderId).HasColumnName("Order_Id");

                entity.Property(e => e.CustomerId).HasColumnName("Customer_Id");

                entity.Property(e => e.IsDelivered).HasMaxLength(50);

                entity.Property(e => e.IsPaid).HasMaxLength(50);

                entity.Property(e => e.OrderDate)
                    .HasColumnType("datetime")
                    .HasColumnName("Order_Date");

                entity.Property(e => e.OrderNo).HasColumnName("Order_No");

                entity.Property(e => e.OrderTotal)
                    .HasColumnType("decimal(18, 2)")
                    .HasColumnName("Order_Total");

                entity.Property(e => e.PaymentType)
                    .HasMaxLength(10)
                    .IsFixedLength(true);

                entity.Property(e => e.PickUpTime)
                    .HasColumnType("datetime")
                    .HasColumnName("PickUp_Time");

                entity.HasOne(d => d.Customer)
                    .WithMany(p => p.Orders)
                    .HasForeignKey(d => d.CustomerId)
                    .HasConstraintName("FK_Order_Customer");
            });

            modelBuilder.Entity<OrderDetail>(entity =>
            {
                entity.HasKey(e => e.OrderDetailsId);

                entity.ToTable("Order_Details");

                entity.Property(e => e.OrderDetailsId).HasColumnName("Order_Details_Id");

                entity.Property(e => e.Price).HasColumnType("decimal(18, 2)");

                entity.Property(e => e.Subtotal).HasColumnType("decimal(18, 2)");

                entity.HasOne(d => d.Order)
                    .WithMany(p => p.OrderDetails)
                    .HasForeignKey(d => d.OrderId)
                    .HasConstraintName("FK_Order_Details_Order");

                entity.HasOne(d => d.Product)
                    .WithMany(p => p.OrderDetails)
                    .HasForeignKey(d => d.ProductId)
                    .HasConstraintName("FK_Order_Details_Products");
            });

            modelBuilder.Entity<Product>(entity =>
            {
                entity.Property(e => e.Description)
                    .HasMaxLength(400)
                    .IsUnicode(false);

                entity.Property(e => e.Image)
                    .HasMaxLength(2000)
                    .IsUnicode(false);

                entity.Property(e => e.Name)
                    .HasMaxLength(255)
                    .IsUnicode(false);

                entity.Property(e => e.Price).HasColumnType("decimal(18, 2)");

                entity.HasOne(d => d.Department)
                    .WithMany(p => p.Products)
                    .HasForeignKey(d => d.DepartmentId)
                    .HasConstraintName("FK_Products_Department");
            });

            modelBuilder.Entity<SubDepartment>(entity =>
            {
                entity.ToTable("Sub_Department");

                entity.Property(e => e.SubDepartmentId).HasColumnName("Sub_DepartmentID");

                entity.Property(e => e.SubDepartmentName)
                    .IsRequired()
                    .HasMaxLength(50)
                    .HasColumnName("Sub_DepartmentName");
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);

    }
}
