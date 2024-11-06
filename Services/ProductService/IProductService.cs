using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ecommerce_api.DTO.Product;
using ecommerce_api.Models;

namespace ecommerce_api.Services.ProductService
{
    public interface IProductService
    {
        Task<List<Product>> GetProducts(int page, int limit);
        Task<Product?> GetProduct(int id);
        Task<Product> CreateProduct(CreateProductDTO product);
        Task<Product> UpdateProduct(int id, UpdateProductDTO product);
        Task<Product> DeleteProduct(int id);
    }
}