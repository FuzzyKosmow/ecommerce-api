using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ecommerce_api.DTO.User
{

    public class UserDto
    {
        public string Id { get; set; }
        public string FullName { get; set; }
        public string? Address { get; set; }
        public string? Province { get; set; }
        public string? District { get; set; }
        public string? PhoneNumber { get; set; }
        public string? Email { get; set; }
        public string? Role { get; set; }
        public int? TotalOrders { get; set; }

        public double? TotalValue { get; set; }


    }


}