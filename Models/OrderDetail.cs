using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ecommerce_api.Models
{
    public class OrderDetail
    {
        public int Id { get; set; }
        public int OrderId { get; set; }
        public int ProductId { get; set; }
        public int Quantity { get; set; }
        public decimal Price { get; set; }
        public string Color { get; set; }
        public string Storage { get; set; }
        public decimal StorageModifier { get; set; }
        // Navigation Properties
        public Order Order { get; set; }
        public Product Product { get; set; }


    }
}