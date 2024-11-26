using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ecommerce_api.DTO.User
{
    public class UpdateUserDTO
    {
        public string? FullName { get; set; }
        public string? Address { get; set; }
        public string? Province { get; set; }
        public string? District { get; set; }
        public string? PhoneNumber { get; set; }
    }
}