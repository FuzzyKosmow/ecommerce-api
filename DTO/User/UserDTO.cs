using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ecommerce_api.DTO.User
{

    public class UserDto
    {
        public string FullName { get; set; }
        public string Address { get; set; }
        public string PhoneNumber { get; set; }
        public string Email { get; set; }

        public int? TotalOrders { get; set; }

        public double? TotalValue { get; set; }

        public int? TotalProducts { get; set; }

    }


}