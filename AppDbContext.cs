using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using ecommerce_api.Models;
using Microsoft.EntityFrameworkCore;

namespace ecommerce_api
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        public DbSet<Product> Products { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
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
                .Property(p => p.Images)
                .HasConversion(
                    v => string.Join(',', v),
                    v => v.Split(',', StringSplitOptions.RemoveEmptyEntries).ToList()
                );
        }
        public void Seed()
        {
            // Check if there are any existing products in the database
            if (!Products.Any())
            {
                // Sample product data
                var sampleProduct = new Product
                {
                    Name = "Iphone 15 Plus 512GB",
                    Price = 30890000,
                    DiscountPrice = 28890000,
                    Rating = 4.5f,
                    Availability = true,
                    Colors = new List<string> { "Pink", "Blue", "White", "Green", "Yellow" },
                    StorageOptions = new List<string> { "128GB", "256GB", "512GB" },

                    Images = new List<string> { "https://res.cloudinary.com/de0lj9ydr/image/upload/v1729943047/phones/hnlcxkzfry9acrntg3fm.jpg",
                    "https://res.cloudinary.com/de0lj9ydr/image/upload/v1729943047/phones/jda4h9zgfxarw86dg69e.jpg" },
                    Description = "Maintaining the modern square design similar to its predecessors, the iPhone 15 Plus is a perfect choice for users who want a balanced size. It’s not too small like the iPhone 15 or overly expensive like the iPhone 15 Pro Max. Additionally, it comes in three storage options: 128GB/256GB/512GB, offering a wide range of choices for iPhone users.",
                    SpecificationsJson = JsonSerializer.Serialize(new Dictionary<string, string>
                    {
                        { "Operating System", "iOS 17" },
                        { "Mobile Network", "2G, 3G, 4G, 5G" },
                        { "Internal Storage", "256GB" },
                        { "Camera Resolution", "Main Camera: 48MP / f1.6 aperture" },
                        { "SIM Slots", "Dual SIM (nano-SIM and eSIM)" },
                        { "Processor", "Apple A16 Bionic" },
                        { "Display Technology", "Super Retina XDR" },
                        { "Resolution", "2796 x 1290" },
                        { "Screen Size", "6.7 inches" }
                    })
                };

                Products.Add(sampleProduct);
                SaveChanges();
            }
        }
    }



}