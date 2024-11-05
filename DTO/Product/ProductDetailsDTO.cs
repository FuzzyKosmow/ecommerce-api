using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ecommerce_api.DTO.Category;
using ecommerce_api.Models;

namespace ecommerce_api.DTO.Product
{
    public class ProductDetailsDTO
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public decimal Price { get; set; }
        public decimal? Discount_price { get; set; }
        public List<string> Images { get; set; }
        public List<string> Colors { get; set; }
        public List<string> Storage { get; set; }
        public Dictionary<string, string> Specifications { get; set; }
        public string Description { get; set; }
        public decimal Rating { get; set; }
        public List<CategoryDTO> Categories { get; set; }
        public bool IsBestSeller { get; set; }
        public bool IsFeatured { get; set; }
        public bool IsNewArrival { get; set; }
        public DateTime ReleaseDate { get; set; }
    }
}