using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ecommerce_api.DTO.Order
{

    public class CreateOrderDTO
    {
        public string? CustomerId { get; set; }
        public string CustomerName { get; set; }
        public string Province { get; set; }
        public string District { get; set; }
        public string Address { get; set; }
        public string PhoneNumber { get; set; }
        public List<CreateOrderDetailDTO> OrderDetails { get; set; }
        [DefaultValue("Cash")]
        // Allow value Cash, CreditCard, Visa
        [RegularExpression("Cash|CreditCard|Visa")]
        public string PaymentMethod { get; set; }
        [DefaultValue("Standard")]
        [RegularExpression("Standard|Fast|SuperFast")]
        public string ShippingMethod { get; set; }


        // Payment, only valid if payment method is CreditCard or Visa
        public string? CardNumber { get; set; }
        public string? CardHolder { get; set; }
        public string? CardExpireDate { get; set; }
        public string? CardCvv { get; set; }


        // Promo code, different from discount applied by admin
        public string? PromoCodeApplied { get; set; }

    }
}