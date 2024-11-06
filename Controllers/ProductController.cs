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

        // GET /products
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


        // GET /product/{product_id}
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
        [HttpGet("featured")]
        public async Task<ActionResult<IEnumerable<ProductDTO>>> GetFeaturedProducts()
        {
            var products = await _context.Products.Where(p => p.IsFeatured).ToListAsync();
            var productDTOs = _mapper.Map<List<ProductDTO>>(products);
            return Ok(productDTOs);
        }
        [HttpGet("new-arrivals")]
        public async Task<ActionResult<IEnumerable<ProductDTO>>> GetNewArivalProducts()
        {
            var products = await _context.Products.Where(p => p.IsNewArrival).ToListAsync();
            var productDTOs = _mapper.Map<List<ProductDTO>>(products);
            return Ok(productDTOs);
        }
        [HttpGet("bestsellers")]
        public async Task<ActionResult<IEnumerable<ProductDTO>>> GetBestSellerProducts()
        {
            var products = await _context.Products.Where(p => p.IsBestSeller).ToListAsync();
            var productDTOs = _mapper.Map<List<ProductDTO>>(products);
            return Ok(productDTOs);
        }
        //Return product that has discount price
        [HttpGet("deals")]
        public async Task<ActionResult<IEnumerable<ProductDTO>>> GetDeals()
        {
            var products = await _context.Products.Where(p => p.DiscountPrice != null && p.DiscountPrice > 0).ToListAsync();
            var productDTOs = _mapper.Map<List<ProductDTO>>(products);
            return Ok(productDTOs);
        }


        [HttpGet("categories")]
        public async Task<ActionResult<IEnumerable<CategoryDTO>>> GetCategories()
        {
            var categories = await _context.Categories.ToListAsync();
            var categoryDTOs = _mapper.Map<List<CategoryDTO>>(categories);
            return Ok(categoryDTOs);
        }

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