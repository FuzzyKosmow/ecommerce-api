using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ecommerce_api.DTO;
using Microsoft.AspNetCore.Mvc;
using ecommerce_api.Models;
using Microsoft.EntityFrameworkCore;


namespace ecommerce_api.Controllers
{
    [ApiController]
    [Route("api/products")]

    public class ProductsController : ControllerBase
    {
        private readonly AppDbContext _context;

        public ProductsController(AppDbContext context)
        {
            _context = context;
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
            var query = _context.Products.AsQueryable();

            // Add filtering logic
            if (!string.IsNullOrEmpty(category))
            {
                query = query.Where(p => p.Name.Contains(category)); // Simplified example
            }
            if (!string.IsNullOrEmpty(keyword))
            {
                query = query.Where(p => p.Name.Contains(keyword)); // Simplified example
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

            var productDTOs = products.Select(p => new ProductDTO
            {
                Id = p.Id,
                Name = p.Name,
                Price = p.Price,
                Discount_price = p.DiscountPrice,
                Image = p.Images.FirstOrDefault(),

                Colors = p.Colors,
                Storage = p.StorageOptions
            }).ToList();

            return Ok(productDTOs);
        }

        // GET /product/{product_id}
        [HttpGet("{id}")]
        public async Task<ActionResult<ProductDetailsDTO>> GetProduct(int id)
        {
            var product = await _context.Products.FindAsync(id);

            if (product == null)
            {
                return NotFound();
            }

            var productDetailDTO = new ProductDetailsDTO
            {
                Id = product.Id,
                Name = product.Name,
                Price = product.Price,

                Colors = product.Colors,
                Images = product.Images,
                Storage = product.StorageOptions,
                Discount_price = product.DiscountPrice,
                Specifications = product.Specifications,
                Description = product.Description
            };
            return Ok(productDetailDTO);
        }
    }

}