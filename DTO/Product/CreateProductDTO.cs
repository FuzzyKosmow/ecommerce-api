using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ecommerce_api.DTO.Product
{
    public class CreateProductDTO
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public decimal Price { get; set; }
        public string[] Colors { get; set; }
        public string[] StorageOptions { get; set; }
        public decimal[] StorageModifiers { get; set; }
        public string[] Images { get; set; }
        public List<int> CategoryIds { get; set; }
        public decimal ImportPrice { get; set; }
        // Specifications are key-value pairs
        public Dictionary<string, string> Specifications { get; set; }
        public DateTime ReleaseDate { get; set; }
    }
}