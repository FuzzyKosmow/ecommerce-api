using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using ecommerce_api.DTO.Order;
using ecommerce_api.Models;
using ecommerce_api.Services.OrderService;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace ecommerce_api.Services.JWT
{
    public class DatabaseSeeder
    {
        private readonly AppDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IOrderService _orderService;

        public DatabaseSeeder(AppDbContext context, UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager, IOrderService orderService)
        {
            _context = context;
            _userManager = userManager;
            _roleManager = roleManager;
            _orderService = orderService;
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
                    Stock = 1,
                    Colors = new List<string> { "Pink" },
                    StorageOptions = new List<string> { "512GB" },
                    StorageModifiers = new List<decimal> { 1.0m },
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
                    Stock = 1,
                    Colors = new List<string> { "White", "Green", "Yellow" },
                    StorageOptions = new List<string> { "128GB", "256GB", "512GB" },
                    StorageModifiers = new List<decimal> { 1.0m, 1.1m, 1.2m },
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
                    Stock = 1,
                    Colors = new List<string> { "Pink", "Blue", "Yellow" },
                    StorageOptions = new List<string> { "128GB", "256GB", "512GB" },
                    StorageModifiers = new List<decimal> { 1.0m, 1.1m, 1.2m },
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


                var sample4 = new Product
                {
                    Name = "Xiaomi Poco M6 Pro",
                    Price = 30890000,
                    DiscountPrice = 28890000,
                    Rating = 4.5f,
                    Availability = true,
                    Stock = 1,
                    Colors = new List<string> { "Black", "Blue" },
                    StorageOptions = new List<string> { "64GB", "128GB" },
                    StorageModifiers = new List<decimal> { 1.0m, 1.2m },
                    Images = new List<string> { "https://res.cloudinary.com/de0lj9ydr/image/upload/v1732086765/phones/rsuka5u55auiyhrtlqhi.jpg",
                    "https://res.cloudinary.com/de0lj9ydr/image/upload/v1732086765/phones/wa5azmbshkqe8aq2shgs.jpg",
                    "https://res.cloudinary.com/de0lj9ydr/image/upload/v1732086765/phones/grypgpq3mxkmqiopwxza.png",
                    "https://res.cloudinary.com/de0lj9ydr/image/upload/v1732086766/phones/enkilne7lfpfslbathks.png" },
                    Description = "The Xiaomi Poco M6 Pro features a powerful Snapdragon 4 Gen 1 processor, a 6.79-inch display, 50MP dual camera, 5000mAh battery, and more.",
                    SpecificationsJson = JsonSerializer.Serialize(new Dictionary<string, string>
                    {
                        { "Processor", "Snapdragon 4 Gen 1" },
                        { "Display", "6.79 inches" },
                        { "Battery", "5000mAh" },
                        { "Camera", "50MP dual" }
                    }),
                    ReleaseDate = new DateTime(2023, 8, 5),
                    CreatedAt = DateTime.Now,
                    IsNewArrival = true
                };






                var vivo = new Product
                {
                    Name = "Vivo Y36",
                    Price = 8990000,
                    DiscountPrice = 0,
                    Rating = 4.3f,
                    Availability = true,
                    Stock = 1,
                    Colors = new List<string> { "Black", "Gold" },
                    StorageOptions = new List<string> { "128GB" },
                    StorageModifiers = new List<decimal> { 1.0m },
                    Images = new List<string> { "https://res.cloudinary.com/de0lj9ydr/image/upload/v1732090350/phones/bclm1g5oc9xpgsqtl0hx.webp",
                    "https://res.cloudinary.com/de0lj9ydr/image/upload/v1732090350/phones/b8m6bzgm2tzskrobqjme.webp",
                    "https://res.cloudinary.com/de0lj9ydr/image/upload/v1732090350/phones/ukysjhzxc15gp0siditt.webp"
                    },
                    Description = "The Vivo Y36 is a budget-friendly smartphone offering a sleek design and robust features for everyday users. It comes with a 6.64-inch display that provides a vivid and immersive viewing experience. Powered by the Snapdragon 680 processor, the Vivo Y36 is engineered for efficiency and smooth multitasking. Its triple-camera system allows users to capture stunning photos, while the 5000mAh battery ensures that you stay powered throughout the day. This phone also supports 44W FlashCharge, meaning you can recharge in a snap when needed. Other notable features include facial recognition, a side-mounted fingerprint scanner, and dual SIM functionality. It is an excellent choice for users looking for an all-around performer with a focus on essential smartphone experiences.",
                    SpecificationsJson = JsonSerializer.Serialize(new Dictionary<string, string>
                    {
                        { "Display", "6.64-inch FHD+" },
                        { "Processor", "Snapdragon 680" },
                        { "RAM", "8GB" },
                        { "Battery", "5000mAh" },
                        { "Charging", "44W FlashCharge" },
                        { "OS", "Android 13" }
                    }),
                    IsBestSeller = true,
                    ReleaseDate = new DateTime(2024, 10, 12),
                    CreatedAt = DateTime.Now,
                    IsNewArrival = true
                };
                var realme13 = new Product
                {
                    Name = "Realme 13",
                    Price = 19990000,
                    Rating = 4.5f,
                    Availability = true,
                    DiscountPrice = 0,
                    ImportPrice = 15000000,
                    Colors = new List<string> { "Silver", "Blue", "Black" }, // Available colors
                    StorageOptions = new List<string> { "128GB", "256GB" }, // Storage options
                    StorageModifiers = new List<decimal> { 1.0m, 1.2m }, // Storage price modifiers
                    Images = new List<string>
    {
        "https://res.cloudinary.com/de0lj9ydr/image/upload/v1732091392/phones/efcugqlsuehnwno28kxc.webp",
        "https://res.cloudinary.com/de0lj9ydr/image/upload/v1732091392/phones/e58ednlyix3acngncaax.webp",
        "https://res.cloudinary.com/de0lj9ydr/image/upload/v1732091392/phones/qmudw17x7hgfcojefprq.webp",
        "https://res.cloudinary.com/de0lj9ydr/image/upload/v1732091392/phones/zbroyrsihhgjh0fytrov.webp"
    },
                    Description = "The Realme 13 is a cutting-edge smartphone that combines advanced technology with an elegant design. Featuring a 6.7-inch Super AMOLED display with a 120Hz refresh rate, this device provides an immersive viewing experience. Powered by the MediaTek Dimensity 9200 processor, it ensures blazing-fast performance and smooth multitasking. The Realme 13 is equipped with a versatile triple-camera setup, allowing you to capture stunning photos and videos in various conditions. Its 5000mAh battery supports 67W SuperDart charging, giving you more usage time with less downtime. Additionally, the phone includes dual stereo speakers, an in-display fingerprint scanner, and runs on Realme UI 5.0 based on Android 14. Whether you’re gaming, streaming, or working, the Realme 13 is built to exceed expectations.",
                    Specifications = new Dictionary<string, string>
    {
        { "Display", "6.7-inch Super AMOLED, 120Hz" },
        { "Processor", "MediaTek Dimensity 9200" },
        { "RAM", "12GB" },
        { "Battery", "5000mAh" },
        { "Charging", "67W SuperDart" },
        { "OS", "Android 14" },
        { "Audio", "Dual stereo speakers" }
    },
                    IsBestSeller = true, // Marked as bestseller
                    IsFeatured = true, // Featured product
                    ReleaseDate = new DateTime(2024, 11, 5), // Release date
                    CreatedAt = DateTime.Now, // Automatically set to current time
                    IsNewArrival = true // Marked as a new arrival
                };


                await _context.Products.AddRangeAsync(sampleProduct, sample2, sample3, sample4, vivo, realme13);
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
            await CreateOrder(_context);


        }
        public async Task CreateOrder(AppDbContext context)
        {
            // Retrieve the product from the database (assuming it has an Id of 3)
            var product = await context.Products.FindAsync(3);

            if (product == null)
            {
                Console.WriteLine("Product not found");
                return;
            }

            // Determine the price based on DiscountPrice or Price
            decimal effectivePrice = product.DiscountPrice != null && product.DiscountPrice > 0 ? product.DiscountPrice.Value : product.Price;
            // Add to the first user with role User
            var firstUserInDb = await context.Users.FirstOrDefaultAsync(u => u.Email == "user1@gm.com");

            if (firstUserInDb == null)
            {
                Console.WriteLine("User not found");
                return;
            }
            // Create a new order
            var order = new CreateOrderDTO
            {
                CustomerId = firstUserInDb.Id,
                PaymentMethod = "Credit Card",
                OrderDetails = new List<CreateOrderDetailDTO>()
                {
                    new CreateOrderDetailDTO
                    {
                    ProductId = product.Id,
                    Quantity = 2,  // Example quantity
                    Storage = product.StorageOptions[2],
                    Color = product.Colors[1],
                    StorageModifier = product.StorageModifiers[2],
                    }
                },
                Address = "1234 Main St",
                Province = "Ontario",
                District = "Toronto",
                PhoneNumber = "1234567890",
                ShippingMethod = "Standard",
                CardCvv = "123",
                CardExpireDate = "12/25",
                CardHolder = "John Doe",
                CardNumber = "1234567890123456",

            };


            var orderRes = await _orderService.CreateOrderAsync(order);

            // Save changes to the database
            await context.SaveChangesAsync();
            await SeedVoucher();
            Console.WriteLine($"Order created successfully with ID {orderRes.Id} with total {orderRes.Total}, sub total {orderRes.SubTotal}");
        }
        public async Task SeedVoucher()
        {
            if (await _context.Vouchers.CountAsync() == 0)
            {
                var voucher = new Voucher
                {
                    Code = "V12345",
                    Name = "50% off christmas",
                    DiscountPercentage = 50,
                    ExpiryDate = DateTime.Now.AddMonths(1),
                    IsActive = true,
                    Description = "50% off for all products",
                };
                await _context.Vouchers.AddAsync(voucher);
                await _context.SaveChangesAsync();
            }
        }




    }
}