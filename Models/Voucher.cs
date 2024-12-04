using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;

namespace ecommerce_api.Models
{
    /// <summary>
    /// Represents a voucher that can be used to get a discount on an order. By design its applicable to all products.
    /// Can be used once by a user.
    /// </summary>
    public class Voucher

    {
        public int Id { get; set; }
        public string Code { get; set; }
        public decimal DiscountPercentage { get; set; }
        public string Description { get; set; }
        public string Name { get; set; }
        [DefaultValue("GETDATE() + 365")]
        public DateTime ExpiryDate { get; set; }
        [DefaultValue(false)]
        public bool IsUsed { get; set; }
        public string? UserId { get; set; }
        [DefaultValue(true)]
        public bool IsActive { get; set; }
        // Navigation Properties
        public ApplicationUser? User { get; set; }

    }
}