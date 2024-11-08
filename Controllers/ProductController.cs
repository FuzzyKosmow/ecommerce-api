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
        private readonly int defaultDayCountAsNewArrival = 7;

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
    [FromQuery] string? sort,
    [FromQuery] int page = 1,
    [FromQuery] int limit = 10)
        {
            var query = _context.Products
                .Include(p => p.Categories)
                .AsQueryable();

            // Filter by category
            if (!string.IsNullOrEmpty(category))
            {
                query = query.Where(p => p.Categories.Any(c => c.Name.ToLower() == category.ToLower()));
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
                case "price_asc":
                    productList = productList.OrderBy(p => p.Price).ToList();
                    break;
                case "price_desc":
                    productList = productList.OrderByDescending(p => p.Price).ToList();
                    break;
                case "discount":
                    productList = productList
                        .Where(p => p.DiscountPrice.HasValue)
                        .OrderBy(p => p.DiscountPrice)
                        .ToList();
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
        [Authorize(Roles = "Admin")]
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


    }

}