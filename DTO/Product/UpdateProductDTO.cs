using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ecommerce_api.DTO.Product
{
    public class UpdateProductDTO
    {
        public string? Name { get; set; }
        public decimal? Price { get; set; }
        public decimal? DiscountPrice { get; set; }
        public float? Rating { get; set; }
        public bool? Availability { get; set; }
        public decimal? ImportPrice { get; set; }
        public List<string>? Colors { get; set; }
        public List<string>? StorageOptions { get; set; }
        public List<string>? Images { get; set; }
        public string? Description { get; set; }
        public Dictionary<string, string>? Specifications { get; set; }
        public bool? IsBestSeller { get; set; }
        public bool? IsFeatured { get; set; }
        public DateTime? ReleaseDate { get; set; }
        public bool? IsNewArrival { get; set; }
    }
}