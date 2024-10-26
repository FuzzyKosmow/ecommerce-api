using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ecommerce_api.DTO
{

    public class ProductDTO
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public decimal Price { get; set; }
        public decimal? Discount_price { get; set; }
        public string Image { get; set; }
        public List<string> Colors { get; set; }
        public List<string> Storage { get; set; }
    }



}