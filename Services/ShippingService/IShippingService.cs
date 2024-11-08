using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ecommerce_api.Services.ShippingService
{
    public interface IShippingService
    {
        Task<decimal> CalculateShippingCost(string province, string district, string address, string method);
        Task<string> GetTrackingNumber();
    }
}