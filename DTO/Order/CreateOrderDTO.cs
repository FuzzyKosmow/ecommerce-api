using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;

namespace ecommerce_api.DTO.Order
{

    public class CreateOrderDTO
    {
        public string? CustomerId { get; set; }
        public List<CreateOrderDetailDTO> OrderDetails { get; set; }
        [DefaultValue("Cash")]
        public string PaymentMethod { get; set; }
    }
}