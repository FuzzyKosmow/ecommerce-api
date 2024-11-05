using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using ecommerce_api.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace ecommerce_api.Services.JWT
{
    public class DatabaseSeeder
    {
        private readonly AppDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public DatabaseSeeder(AppDbContext context, UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            _context = context;
            _userManager = userManager;
            _roleManager = roleManager;
        }
        public async Task EnsureRoles()
        {
            if (!await _roleManager.RoleExistsAsync("User"))
            {
                await _roleManager.CreateAsync(new IdentityRole("User"));
            }

            if (!await _roleManager.RoleExistsAsync("Admin"))
            {
                await _roleManager.CreateAsync(new IdentityRole("Admin"));
            }
        }

        public async Task SeedDatabase()
        {
            await EnsureRoles();
            if (!await _roleManager.RoleExistsAsync("User") || !await _roleManager.RoleExistsAsync("Admin"))
            {
                throw new Exception("Roles were not created successfully.");
            }
            // Seed categories
            if (await _context.Categories.CountAsync() == 0)
            {
                var categories = new List<Category>
                {
                    new Category { Name = "Phones" },
                    new Category { Name = "Laptops" },
                    new Category { Name = "Tablets" },
                    new Category { Name = "Smartwatches" },
                    new Category { Name = "Accessories" },
                    new Category { Name = "Android" },
                    new Category { Name = "iOS" },
                    new Category { Name = "Windows" },
                };
                await _context.Categories.AddRangeAsync(categories);
            }

            // Check if there are any existing products in the database
            if (await _context.Products.CountAsync() == 0)
            {
                // Sample product data
                var sampleProduct = new Product
                {
                    Name = "Samsung 15 Plus 512GB",
                    Price = 30890000,
                    DiscountPrice = 0,
                    Rating = 4.5f,
                    Availability = true,
                    Colors = new List<string> { "Pink" },
                    StorageOptions = new List<string> { "512GB" },

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

                //Add products 3 times
                var sample2 = new Product
                {
                    Name = "Iphone 15 Plus 512GB",
                    Price = 30890000,
                    DiscountPrice = 0,
                    Rating = 4.5f,
                    Availability = true,
                    Colors = new List<string> { "White", "Green", "Yellow" },
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
                    }),
                    IsNewArrival = true
                };

                var sample3 = new Product
                {
                    Name = "Google 15 Plus 512GB",
                    Price = 30890000,
                    DiscountPrice = 28890000,
                    Rating = 4.5f,
                    Availability = true,
                    Colors = new List<string> { "Pink", "Blue", "Yellow" },
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
                    }),
                    IsBestSeller = true,
                    IsFeatured = true

                };

                await _context.Products.AddRangeAsync(sampleProduct, sample2, sample3);
                await _context.SaveChangesAsync();

                // Assign random categories to each product
                var categories = await _context.Categories.ToListAsync();
                var productsToAssign = await _context.Products.ToListAsync();

                foreach (var product in productsToAssign)
                {
                    var random = new Random();
                    var randomCategories = categories.OrderBy(x => random.Next()).Take(2).ToList();
                    product.Categories = randomCategories;

                }
                await _context.SaveChangesAsync();


            }

            // Add user

            var userSample = new ApplicationUser
            {
                Email = "user1@gm.com",
                FullName = "User 1",
                PhoneNumber = "1234567890",
                Address = "1234 Main St",
                UserName = "user1@gm.com",
                EmailConfirmed = true,
                NormalizedEmail = "user1@gm.com".ToUpper(),

            };
            var existingUserByEmail = await _userManager.FindByEmailAsync(userSample.Email);
            var existingUserByUsername = await _userManager.FindByNameAsync(userSample.UserName);
            if (existingUserByEmail != null || existingUserByUsername != null)
            {
                return;
            }
            else
            {
                var userCreationResult = await _userManager.CreateAsync(userSample, "Password123!");
                if (!userCreationResult.Succeeded)
                {
                    throw new Exception("User creation failed: " + string.Join(", ", userCreationResult.Errors.Select(e => e.Description)));
                }

                await _userManager.AddToRoleAsync(userSample, "User");

            }
            await _context.SaveChangesAsync();


            // Add admin
            var adminSample = new ApplicationUser
            {
                Email = "admin@admin.com",
                FullName = "Admin",
                PhoneNumber = "1234567890",
                Address = "1234 Main St",
                UserName = "admin@admin.com",
                EmailConfirmed = true,
            };
            var existingAdminByEmail = await _userManager.FindByEmailAsync(adminSample.Email);
            var existingAdminByUsername = await _userManager.FindByNameAsync(adminSample.UserName);
            if (existingAdminByEmail != null || existingAdminByUsername != null)
            {
                return;
            }
            else
            {
                var adminCreationResult = await _userManager.CreateAsync(adminSample, "Password123!");
                if (!adminCreationResult.Succeeded)
                {
                    throw new Exception("Admin creation failed: " + string.Join(", ", adminCreationResult.Errors.Select(e => e.Description)));
                }

                await _userManager.AddToRoleAsync(adminSample, "Admin");

            }
            await _context.SaveChangesAsync();

            Console.WriteLine("Database seeded successfully");
            // Promotion
            // Create a promotion that apply to id 1,2
            // Discount is 50%, valid until 1 month from now
            if (await _context.Promotions.CountAsync() == 0)
            {
                var promotion = new Promotion
                {
                    Name = "50% off for all products",
                    DiscountPercentage = 50,
                    ValidUntil = DateTime.Now.AddMonths(1),
                    IsActive = true,
                    ApplicableProductIds = new List<int> { 1, 2 }
                };
                await _context.Promotions.AddAsync(promotion);
                await _context.SaveChangesAsync();
            }


        }
    }
}
