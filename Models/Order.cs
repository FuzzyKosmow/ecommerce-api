using System;
using System.Collections.Generic;
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

        //Ignore
        public decimal TotalPrice => OrderDetails?.Sum(od => od.Quantity * od.Price) ?? 0;
        public ICollection<OrderDetail> OrderDetails { get; set; }
    }
}