using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ecommerce_api.DTO.Order
{
    /// <summary>
    /// Used to update order. Allow only change address, province, district for normal user
    /// </summary>
    public class UpdateOrderDTO
    {

        public string? CustomerId { get; set; }
        public string? Province { get; set; }
        public string? District { get; set; }
        public string? Address { get; set; }
        public List<CreateOrderDetailDTO>? OrderDetails { get; set; }

        public string? PaymentMethod { get; set; }

        public string? Status { get; set; }
    }
}