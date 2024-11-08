using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;

namespace ecommerce_api.Models
{
    public class Order
    {

        public int Id { get; set; }
        public string? UserId { get; set; }
        public DateTime OrderDate { get; set; }
        public string Status { get; set; }
        public string PaymentMethod { get; set; }
        // Address
        public string Province { get; set; }
        public string District { get; set; }
        public string Address { get; set; }
        public string PhoneNumber { get; set; }
        // Shipping
        public string ShippingMethod { get; set; }
        public decimal ShippingFee { get; set; }
        public string TrackingID { get; set; }

        // Tax. Is always 10% of total product (before)
        public decimal Tax { get; set; }

        // Payment, only valid if payment method is CreditCard or Visa
        public string? CardNumber { get; set; }
        public string? CardHolder { get; set; }
        public string? CardExpireDate { get; set; }
        public string? CardCvv { get; set; }

        // Promo code, different from discount applied by admin
        public string? PromoCodeApplied { get; set; }
        // Its value, not percentage
        public decimal? PromoCodeDiscount { get; set; }







        //Ignore
        // SubTotal = sum of detail price * quantity * storage modifier 
        public decimal SubTotal => OrderDetails.Sum(x => x.Price * x.Quantity * x.StorageModifier);
        public decimal Total => SubTotal + ShippingFee + Tax - (PromoCodeDiscount ?? 0);
        public ICollection<OrderDetail> OrderDetails { get; set; }
    }
}