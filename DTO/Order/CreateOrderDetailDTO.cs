using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ecommerce_api.DTO.Order
{
    public class CreateOrderDetailDTO
    {
        //Product id, quantity
        public int ProductId { get; set; }
        public int Quantity { get; set; }
        public string Color { get; set; }
        public string Storage { get; set; }
        public decimal StorageModifier { get; set; }


    }
}