using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ecommerce_api.DTO.Shipping
{
    public class ShippingRequest
    {
        public string Province { get; set; }
        public string District { get; set; }
        public string Address { get; set; }
        public string Method { get; set; }
    }
}