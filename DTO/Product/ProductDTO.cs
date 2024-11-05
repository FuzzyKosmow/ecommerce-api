using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using ecommerce_api.DTO.Category;
using ecommerce_api.Models;

namespace ecommerce_api.DTO.Product
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
        // Implement rating later
        public decimal Rating { get; set; }
        public List<CategoryDTO> Categories { get; set; }
        public bool Is_bestseller { get; set; }
        public bool Is_featured { get; set; }
        public bool Is_new_arrival { get; set; }
        public DateTime Release_date { get; set; }


    }



}