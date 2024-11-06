using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ecommerce_api.DTO.Order
{
    public class UpdateOrderDTO
    {

        public string? CustomerId { get; set; }
        public List<CreateOrderDetailDTO>? OrderDetails { get; set; }

        public string? PaymentMethod { get; set; }

        public string? Status { get; set; }
    }
}