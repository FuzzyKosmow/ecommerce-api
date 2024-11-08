using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;

namespace ecommerce_api.DTO.Voucher
{
    public class CreateVoucherDTO
    {
        public string Name { get; set; }
        public decimal DiscountPercentage { get; set; }
        public string Description { get; set; }
        [DefaultValue("GETDATE() + 365")]
        public DateTime ExpiryDate { get; set; }
    }
}