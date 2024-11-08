using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ecommerce_api.DTO.Category;
using ecommerce_api.Models;

namespace ecommerce_api.DTO.Product
{   /// <summary>
/// Data Transfer Object for Product Details used on details page
/// </summary>
    public class ProductDetailsDTO
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public decimal Price { get; set; }
        public decimal? Discount_price { get; set; }
        public List<string> Images { get; set; }
        public List<string> Colors { get; set; }
        public List<string> Storage { get; set; }
        public List<decimal> StorageModifiers { get; set; }
        public Dictionary<string, string> Specifications { get; set; }
        public string Description { get; set; }
        public decimal Rating { get; set; }
        public List<CategoryDTO> Categories { get; set; }
        public bool Is_bestseller { get; set; }
        public bool Is_featured { get; set; }
        public bool Is_new_arrival { get; set; }
        public DateTime Release_date { get; set; }
    }
}