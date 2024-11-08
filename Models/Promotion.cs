using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ecommerce_api.Models
{
    /// <summary>
    ///  Used to manage discount applied to products, reflect in discount price. Different from promo code aka coupon
    /// </summary>
    public class Promotion
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public decimal DiscountPercentage { get; set; }
        public DateTime ValidUntil { get; set; }
        public bool IsActive { get; set; }
        public List<int> ApplicableProductIds { get; set; }

        public List<Product> Products { get; set; }
    }

}