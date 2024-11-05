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


namespace ecommerce_api.Controllers
{
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

        // GET /products
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ProductDTO>>> GetProducts(
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

            // Add filtering logic
            if (!string.IsNullOrEmpty(category))
            {
                query = query.Where(p => p.Categories.Any(c => c.Name == category));
            }
            if (!string.IsNullOrEmpty(keyword))
            {
                query = query.Where(p => p.Name.Contains(keyword));
            }
            if (price_min != null)
            {
                query = query.Where(p => p.Price >= price_min);
            }
            if (price_max != null)
            {
                query = query.Where(p => p.Price <= price_max);
            }
            if (!string.IsNullOrEmpty(color))
            {
                query = query.Where(p => p.Colors.Contains(color));
            }
            if (!string.IsNullOrEmpty(storage))
            {
                query = query.Where(p => p.StorageOptions.Contains(storage));
            }




            // Sorting
            if (sort == "price_asc")
            {
                query = query.OrderBy(p => p.Price);
            }
            else if (sort == "price_desc")
            {
                query = query.OrderByDescending(p => p.Price);
            }
            else if (sort == "discount")
            {
                query = query.Where(p => p.DiscountPrice != null).OrderBy(p => p.DiscountPrice);
            }

            // Pagination
            var products = await query.Skip((page - 1) * limit).Take(limit).ToListAsync();

            var productDTOs = _mapper.Map<List<ProductDTO>>(products);
            //Turn in to res that has .products 
            var mapped = new ProductListDTO
            {
                Products = productDTOs,
                Total = productDTOs.Count
            };
            return Ok(mapped);
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
    }

}