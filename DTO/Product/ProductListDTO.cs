using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ecommerce_api.DTO.Product
{
    public class ProductListDTO
    {
        public List<ProductDTO> Products { get; set; }
        public int Total { get; set; }
    }
}