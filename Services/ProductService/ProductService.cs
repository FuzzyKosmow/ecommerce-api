using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using ecommerce_api.DTO.Product;
using ecommerce_api.Models;

namespace ecommerce_api.Services.ProductService
{
    /// <summary>
    /// Service for managing products. Quite different from one in the controller where it's used for filtering and pagination.
    /// </summary>
    public class ProductService : IProductService
    {
        private readonly AppDbContext _context;
        private readonly IMapper _mapper;
        public ProductService(AppDbContext context, IMapper mapper)
        {
            _mapper = mapper;
            _context = context;
        }
        public async Task<Product> CreateProduct(CreateProductDTO product)
        {
            // Map the DTO to the model
            var productModel = _mapper.Map<Product>(product);
            // Add the product to the database
            await _context.Products.AddAsync(productModel);
            await _context.SaveChangesAsync();
            return productModel;
        }

        public async Task<Product> DeleteProduct(int id)
        {
            var product = await _context.Products.FindAsync(id);
            if (product == null)
            {
                return null;
            }
            _context.Products.Remove(product);
            await _context.SaveChangesAsync();
            return product;
        }

        public async Task<Product?> GetProduct(int id)
        {
            var product = await _context.Products.FindAsync(id);
            return product;
        }

        public async Task<List<Product>> GetProducts(int page, int limit)
        {
            return _context.Products.Skip((page - 1) * limit).Take(limit).ToList();
        }

        public async Task<Product> UpdateProduct(int id, UpdateProductDTO product)
        {
            var productModel = await _context.Products.FindAsync(id);
            if (productModel == null)
            {
                return null;
            }
            // Map the DTO to the model, updating only non-null properties
            _mapper.Map(product, productModel);
            await _context.SaveChangesAsync();
            return productModel;
        }


    }
}