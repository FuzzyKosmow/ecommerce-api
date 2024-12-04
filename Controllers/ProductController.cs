using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ecommerce_api.DTO;
using Microsoft.AspNetCore.Mvc;
using ecommerce_api.Models;
using Microsoft.EntityFrameworkCore;
using ecommerce_api.DTO.Product;
using ecommerce_api.DTO.Category;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;


namespace ecommerce_api.Controllers
{
    /// <summary>
    /// Controller for handling products. Used for both customer and admin
    /// </summary>
    [ApiController]
    [Route("api/products")]
    public class ProductsController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly IMapper _mapper;


        public ProductsController(AppDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        /// <summary>
        /// Get a list of products with optional filters and pagination
        /// </summary>
        /// <param name="category">
        ///     Each product has a list of category that it belongs to. This filter will return products that belong to the specified category
        ///     EXAMPLE: Iphone, Samsung, Android, ETC. Tagging a product with multiple categories is possible
        /// </param>
        /// <param name="keyword">
        ///     Search for products that contain the keyword in their name
        /// </param>
        /// <param name="price_min">
        ///     Filter products with price greater than or equal to this value
        /// </param>
        /// <param name="price_max">
        ///     Filter products with price less than or equal to this value
        /// </param>
        /// <param name="color">
        ///     Filter products that have the specified color(s)
        /// </param>
        /// <param name="storage">
        ///     Filter products that have the specified storage option(s)
        /// </param>
        /// <param name="sort">
        ///     Sort the products by price in ascending or descending order, or by discount
        /// </param>
        /// <param name="page">
        ///     The page number for pagination
        /// </param>
        /// <param name="limit">
        ///     The number of products per page
        /// </param>
        /// <returns>
        ///     200: The list of products, total number of products before pagination. Even if empty, it will return an empty list
        /// </returns>
        [HttpGet]
        public async Task<ActionResult<ProductListDTO>> GetProducts(
    [FromQuery] string? category,
    [FromQuery] string? keyword,
    [FromQuery] decimal? price_min,
    [FromQuery] decimal? price_max,
    [FromQuery] string? color,
    [FromQuery] string? storage,
    [FromQuery] string? sort, // Can be 4 values : "price-high-to-low", "price-low-to-high", "most-popular", "newest-arrivals"
    [FromQuery] int page = 1,
    [FromQuery] int limit = 10)
        {
            var query = _context.Products
                .Include(p => p.Categories)
                .AsQueryable();


            if (!string.IsNullOrEmpty(category))
            {
                var categoryList = category.Split(',', StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries).ToList();

                query = query.Where(p => p.Categories.Any(c => categoryList.Contains(c.Name.ToLower())));
            }

            // Filter by keyword
            if (!string.IsNullOrEmpty(keyword))
            {
                query = query.Where(p => p.Name.ToLower().Contains(keyword.ToLower()));
            }

            // Filter by price range
            if (price_min.HasValue)
            {
                query = query.Where(p => p.Price >= price_min);
            }
            if (price_max.HasValue)
            {
                query = query.Where(p => p.Price <= price_max);
            }

            // Fetch data from database, then filter colors and storage on the client side
            var productList = await query.AsNoTracking().ToListAsync();

            // Filter by color(s)
            if (!string.IsNullOrEmpty(color))
            {
                var colorList = color.Split(',', StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries).ToList();
                productList = productList
                    .Where(p => p.Colors.Any(c => colorList.Contains(c.ToLower())))
                    .ToList();
            }

            // Filter by storage option(s)
            if (!string.IsNullOrEmpty(storage))
            {
                var storageList = storage.Split(',', StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries).ToList();
                productList = productList
                    .Where(p => p.StorageOptions.Any(s => storageList.Contains(s.ToLower())))
                    .ToList();
            }

            // Sorting
            switch (sort)
            {
                case "price-high-to-low":
                    // Compare price1, if discount price is not null or > 0 , then price1 = discount price, else price1 = price
                    productList = productList.OrderByDescending(p => p.DiscountPrice != null && p.DiscountPrice > 0 ? p.DiscountPrice : p.Price).ToList();
                    break;
                case "price-low-to-high":
                    // Similar to above
                    productList = productList.OrderBy(p => p.DiscountPrice != null && p.DiscountPrice > 0 ? p.DiscountPrice : p.Price).ToList();
                    break;
                case "most-popular":
                    // Todo: Implement a better way to calculate the most popular products
                    productList = productList.OrderByDescending(p => p.Price).ToList();
                    break;
                case "newest-arrivals":
                    productList = productList.OrderByDescending(p => p.ReleaseDate).ToList();
                    break;
                default:
                    break;
            }

            // Pagination
            var pagedProducts = productList
                .Skip((page - 1) * limit)
                .Take(limit)
                .ToList();

            // Map to ProductDTO
            var productDTOs = _mapper.Map<List<ProductDTO>>(pagedProducts);

            // Prepare the final response
            var result = new ProductListDTO
            {
                Products = productDTOs,
                Total = productList.Count // Total before pagination
            };

            return Ok(result);
        }


        /// <summary>
        /// Get a product by id
        /// </summary>
        /// <param name="id"></param>
        /// <returns>
        ///     200: The product
        ///     404: Not found if the product does not exist
        ///     500: Internal server error if there is an exception
        /// </returns>
        [HttpGet("{id}")]
        public async Task<ActionResult<ProductDetailsDTO>> GetProduct(int id)
        {
            var product = await _context.Products.Include(p => p.Categories).FirstOrDefaultAsync(p => p.Id == id);

            if (product == null)
            {
                return NotFound();
            }
            var productDetailDTO = _mapper.Map<ProductDetailsDTO>(product);
            return Ok(productDetailDTO);
        }
        /// <summary>
        /// Get featured products. Used on home page
        ///    Featured products are products that are marked as featured by admin
        /// </summary>
        /// <returns>
        ///     200: The list of featured products
        ///     500: Internal server error if there is an exception
        /// </returns>
        [HttpGet("featured")]
        public async Task<ActionResult<IEnumerable<ProductDTO>>> GetFeaturedProducts()
        {
            var products = await _context.Products.Where(p => p.IsFeatured).ToListAsync();
            var productDTOs = _mapper.Map<List<ProductDTO>>(products);
            return Ok(productDTOs);
        }
        /// <summary>
        /// Get new arrival products. Used on home page
        ///    New arrival products are products that are released recently or is marked as new arrival by admin
        /// </summary>
        /// <returns>
        ///     200: The list of new arrival products
        ///     500: Internal server error if there is an exception
        /// </returns>
        [HttpGet("new-arrivals")]
        public async Task<ActionResult<IEnumerable<ProductDTO>>> GetNewArivalProducts()
        {
            var products = await _context.Products.Where(p => p.IsNewArrival).ToListAsync();
            var productDTOs = _mapper.Map<List<ProductDTO>>(products);
            return Ok(productDTOs);
        }
        /// <summary>
        ///     Get best seller products. Used on home page
        ///     Best seller products are products that are sold the most, or is marked as best seller by admin
        /// </summary>
        /// <returns>
        ///     200: The list of best seller products
        ///     500: Internal server error if there is an exception
        /// </returns>
        [HttpGet("bestsellers")]
        public async Task<ActionResult<IEnumerable<ProductDTO>>> GetBestSellerProducts()
        {
            var products = await _context.Products.Where(p => p.IsBestSeller).ToListAsync();
            var productDTOs = _mapper.Map<List<ProductDTO>>(products);
            return Ok(productDTOs);
        }
        /// <summary>
        ///   Get deals. Used on home page. Deals are product that have discount price
        /// </summary>
        /// <returns>
        ///     200: The list of deals
        ///     500: Internal server error if there is an exception
        /// </returns>
        [HttpGet("deals")]
        public async Task<ActionResult<IEnumerable<ProductDTO>>> GetDeals()
        {
            var products = await _context.Products.Where(p => p.DiscountPrice != null && p.DiscountPrice > 0).ToListAsync();
            var productDTOs = _mapper.Map<List<ProductDTO>>(products);
            return Ok(productDTOs);
        }

        /// <summary>
        ///     Get categories
        /// </summary>
        /// <returns>
        ///     200: The list of categories
        ///     500: Internal server error if there is an exception
        /// </returns>
        [HttpGet("categories")]
        public async Task<ActionResult<IEnumerable<CategoryDTO>>> GetCategories()
        {
            var categories = await _context.Categories.ToListAsync();
            var categoryDTOs = _mapper.Map<List<CategoryDTO>>(categories);
            return Ok(categoryDTOs);
        }

        // Same as normal search, but for admin, return AdminProductDTO which has more details
        [HttpGet("admin-get")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<List<Product>>> GetProductsAdmin(
                [FromQuery] string? category,
    [FromQuery] string? keyword,
    [FromQuery] decimal? price_min,
    [FromQuery] decimal? price_max,
    [FromQuery] string? color,
    [FromQuery] string? storage,
    [FromQuery] string? sort, // Can be 4 values : "price-high-to-low", "price-low-to-high", "most-popular", "newest-arrivals"
    [FromQuery] int page = 1,
    [FromQuery] int limit = 10)
        {
            var query = _context.Products
                   .Include(p => p.Categories)
                   .AsQueryable();


            if (!string.IsNullOrEmpty(category))
            {
                var categoryList = category.Split(',', StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries).ToList();

                query = query.Where(p => p.Categories.Any(c => categoryList.Contains(c.Name.ToLower())));
            }

            // Filter by keyword
            if (!string.IsNullOrEmpty(keyword))
            {
                query = query.Where(p => p.Name.ToLower().Contains(keyword.ToLower()));
            }

            // Filter by price range
            if (price_min.HasValue)
            {
                query = query.Where(p => p.Price >= price_min);
            }
            if (price_max.HasValue)
            {
                query = query.Where(p => p.Price <= price_max);
            }

            // Fetch data from database, then filter colors and storage on the client side
            var productList = await query.AsNoTracking().ToListAsync();

            // Filter by color(s)
            if (!string.IsNullOrEmpty(color))
            {
                var colorList = color.Split(',', StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries).ToList();
                productList = productList
                    .Where(p => p.Colors.Any(c => colorList.Contains(c.ToLower())))
                    .ToList();
            }

            // Filter by storage option(s)
            if (!string.IsNullOrEmpty(storage))
            {
                var storageList = storage.Split(',', StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries).ToList();
                productList = productList
                    .Where(p => p.StorageOptions.Any(s => storageList.Contains(s.ToLower())))
                    .ToList();
            }

            // Sorting
            switch (sort)
            {
                case "price-high-to-low":
                    // Compare price1, if discount price is not null or > 0 , then price1 = discount price, else price1 = price
                    productList = productList.OrderByDescending(p => p.DiscountPrice != null && p.DiscountPrice > 0 ? p.DiscountPrice : p.Price).ToList();
                    break;
                case "price-low-to-high":
                    // Similar to above
                    productList = productList.OrderBy(p => p.DiscountPrice != null && p.DiscountPrice > 0 ? p.DiscountPrice : p.Price).ToList();
                    break;
                case "most-popular":
                    // Todo: Implement a better way to calculate the most popular products
                    productList = productList.OrderByDescending(p => p.Price).ToList();
                    break;
                case "newest-arrivals":
                    productList = productList.OrderByDescending(p => p.ReleaseDate).ToList();
                    break;
                default:
                    break;
            }

            // Pagination
            var pagedProducts = productList
                .Skip((page - 1) * limit)
                .Take(limit)
                .ToList();



            return Ok(pagedProducts);

        }

        /// <summary>
        /// Create a product. Only admin can create a product
        /// </summary>
        /// <param name="productDTO">
        ///     Name: The name of the product
        ///     Price: The price of the product
        ///     Description: The description of the product
        ///     Colors: The colors of the product
        ///     StorageOptions: The storage options of the product
        ///     StorageModifiers: The storage modifiers of the product
        ///     Images: The images of the product
        ///     CategoryIds: The categories that the product belongs to
        ///     ImportPrice: The import price of the product
        ///     Specifications: The specifications of the product
        ///     ReleaseDate: The release date of the product
        /// </param>
        /// <returns>
        ///     201: The created product
        ///     400: Bad request if the product is invalid
        ///     401: Unauthorized if the user is not an admin
        ///     500: Internal server error if there is an exception
        /// </returns>
        [HttpPost]
        public async Task<ActionResult<ProductDTO>> CreateProduct(CreateProductDTO productDTO)
        {
            var product = _mapper.Map<Product>(productDTO);
            var categories = await _context.Categories.Where(c => productDTO.CategoryIds.Contains(c.Id)).ToListAsync();
            product.CreatedAt = DateTime.Now;
            _context.Products.Add(product);
            await _context.SaveChangesAsync();
            //Link product to categories
            if (categories.Count > 0)
            {
                product.Categories = categories;
            }
            await _context.SaveChangesAsync();
            var productDTOToReturn = _mapper.Map<ProductDTO>(product);
            return CreatedAtAction(nameof(GetProduct), new { id = product.Id }, productDTOToReturn);
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<ProductDTO>> UpdateProduct(int id, UpdateProductDTO productDTO)
        {
            // Find the product with its related categories
            var product = await _context.Products.Include(p => p.Categories).FirstOrDefaultAsync(p => p.Id == id);

            if (product == null)
            {
                return NotFound();
            }

            // Map updated properties from DTO to the product entity
            _mapper.Map(productDTO, product);

            // Update specific properties manually if required ( lists)
            _context.Entry(product).Property(p => p.Colors).IsModified = true;
            _context.Entry(product).Property(p => p.StorageOptions).IsModified = true;
            _context.Entry(product).Property(p => p.StorageModifiers).IsModified = true;
            _context.Entry(product).Property(p => p.Images).IsModified = true;

            Console.WriteLine("Best seller status: " + product.IsBestSeller);

            // Handle category updates (tags)
            var currentCategoryIds = product.Categories.Select(c => c.Id).ToList();
            var newCategoryIds = productDTO.CategoryIds ?? new List<int>();

            // Step 1: Remove categories that are no longer in the incoming list
            var categoriesToRemove = product.Categories.Where(c => !newCategoryIds.Contains(c.Id)).ToList();
            foreach (var categoryToRemove in categoriesToRemove)
            {
                product.Categories.Remove(categoryToRemove);
            }

            // Step 2: Add new categories (tags) that don't exist
            foreach (var categoryId in newCategoryIds)
            {
                if (!currentCategoryIds.Contains(categoryId))
                {
                    // Find the category in the database, or create a new one if it doesn't exist
                    var category = await _context.Categories.FirstOrDefaultAsync(c => c.Id == categoryId);
                    if (category == null)
                    {
                        // Create a new category if not found
                        category = new Category { Id = categoryId };

                        await _context.Categories.AddAsync(category);
                    }
                    product.Categories.Add(category);
                }
            }

            // Save the changes to the database
            await _context.SaveChangesAsync();

            // Map the updated product back to DTO for response
            var productDTOToReturn = _mapper.Map<ProductDTO>(product);
            Console.WriteLine("Best seller status after update: " + product.IsBestSeller);

            return Ok(productDTOToReturn);
        }

        // Add new category. Take name
        [HttpPost("category")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<CategoryDTO>> CreateCategory([FromBody] string name)
        {
            // Check if new name exists
            var category = await _context.Categories.FirstOrDefaultAsync(c => c.Name == name);
            if (category != null)
            {
                return BadRequest("Category already exists");
            }
            category = new Category { Name = name };
            await _context.Categories.AddAsync(category);
            await _context.SaveChangesAsync();
            var categoryDTO = _mapper.Map<CategoryDTO>(category);
            return CreatedAtAction(nameof(GetCategories), categoryDTO);
        }

        // Update category name
        [HttpPut("category/{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<CategoryDTO>> UpdateCategory(
            int id,
            [FromBody] string newName)
        {
            var category = await _context.Categories.FirstOrDefaultAsync(c => c.Id == id);
            if (category == null)
            {
                return NotFound();
            }
            category.Name = newName;
            await _context.SaveChangesAsync();
            var categoryDTO = _mapper.Map<CategoryDTO>(category);
            return Ok(categoryDTO);
        }

        // Delete category. Also delete the category from all products (joint table)
        [HttpDelete("category/{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult> DeleteCategory(int id)
        {
            // First delete the category from all products
            var products = await _context.Products.Include(p => p.Categories).Where(p => p.Categories.Any(c => c.Id == id)).ToListAsync();
            foreach (var product in products)
            {
                var cat = product.Categories.FirstOrDefault(c => c.Id == id);
                product.Categories.Remove(cat);
            }
            // Then delete the category
            var category = await _context.Categories.FirstOrDefaultAsync(c => c.Id == id);
            if (category == null)
            {
                return NotFound();
            }
            _context.Categories.Remove(category);
            await _context.SaveChangesAsync();
            return NoContent();

        }





        [HttpDelete("{id}")]

        public async Task<ActionResult> DeleteProduct(int id)
        {
            var product = await _context.Products.FirstOrDefaultAsync(p => p.Id == id);
            if (product == null)
            {
                return NotFound();
            }
            _context.Products.Remove(product);
            await _context.SaveChangesAsync();
            return NoContent();
        }

    }
}