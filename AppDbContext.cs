using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text.Json;
using System.Threading.Tasks;
using ecommerce_api.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace ecommerce_api
{
    public class AppDbContext : IdentityDbContext<ApplicationUser>
    {


        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {

        }

        public DbSet<Product> Products { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<ApplicationUser> ApplicationUsers { get; set; }

        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderDetail> OrderDetails { get; set; }


        // Promotions and vouchers
        public DbSet<Promotion> Promotions { get; set; }
        public DbSet<Voucher> Vouchers { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {





            //products, category
            modelBuilder.Entity<Product>()
                .HasMany(p => p.Categories)
                .WithMany(c => c.Products)
                .UsingEntity(j => j.ToTable("ProductCategories"));
            modelBuilder.Entity<Product>()
                    .Ignore(p => p.Specifications);

            modelBuilder.Entity<Product>()
                .Property(p => p.Colors)
                .HasConversion(
                    v => string.Join(',', v),
                    v => v.Split(',', StringSplitOptions.RemoveEmptyEntries).ToList()
                );

            modelBuilder.Entity<Product>()
                .Property(p => p.StorageOptions)
                .HasConversion(
                    v => string.Join(',', v),
                    v => v.Split(',', StringSplitOptions.RemoveEmptyEntries).ToList()
                );
            modelBuilder.Entity<Product>()
                .Property(p => p.StorageModifiers)
                .HasConversion(
                    v => string.Join(',', v),
                    v => v.Split(',', StringSplitOptions.RemoveEmptyEntries).Select(decimal.Parse).ToList()
                );
            modelBuilder.Entity<Product>()
                .Property(p => p.Images)
                .HasConversion(
                    v => string.Join(',', v),
                    v => v.Split(',', StringSplitOptions.RemoveEmptyEntries).ToList()
                );

            modelBuilder.Entity<Product>()
                .Property(p => p.CreatedAt)
                .HasDefaultValueSql("getdate()");


            // promotion
            modelBuilder.Entity<Promotion>()
                .Property(p => p.ApplicableProductIds)
                .HasConversion(
                    v => string.Join(',', v),
                    v => v.Split(',', StringSplitOptions.RemoveEmptyEntries).Select(int.Parse).ToList()
                );


            modelBuilder.Entity<Promotion>()
                .HasMany(p => p.Products);



            //Orders
            modelBuilder.Entity<OrderDetail>()
               .HasKey(od => od.Id);
            modelBuilder.Entity<Order>()
             .HasMany(o => o.OrderDetails)
             .WithOne(od => od.Order)
             .HasForeignKey(od => od.OrderId)
             .OnDelete(DeleteBehavior.Cascade);
            modelBuilder.Entity<Order>()
                .Ignore(o => o.SubTotal);



            // Vouchers. Can be used by any user , but only one. Has a navigation property to the user if used (UserId is not null)
            // Default expiry date is 1 month from creation
            modelBuilder.Entity<Voucher>()
                .Property(v => v.ExpiryDate)
                .HasDefaultValueSql("DATEADD(month, 1, getdate())");
            // UserId is foreign key to ApplicationUser for shortcut to user
            modelBuilder.Entity<Voucher>()
                .HasOne(v => v.User)
                .WithMany(u => u.Vouchers)
                .HasForeignKey(v => v.UserId);





            base.OnModelCreating(modelBuilder);

        }

    }



}