using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ecommerce_api.Services.ShippingService
{
    public class ShippingService : IShippingService
    {
        private readonly List<string> shippingMethods = new List<string> { "Standard", "Fast", "SuperFast" };
        private readonly Dictionary<string, decimal> shippingCosts = new Dictionary<string, decimal>
        {
            //In VND
            { "Standard", 20000 },
            { "Fast", 40000 },
            { "SuperFast", 60000 }
        };
        public async Task<decimal> CalculateShippingCost(string province, string district, string address, string method)
        {
            //Check method is valid
            if (!shippingMethods.Contains(method))
            {
                throw new Exception("Invalid shipping method");
            }
            return shippingCosts[method];

        }
        // 7 random character including number
        public async Task<string> GetTrackingNumber()
        {
            Random random = new Random();
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            return new string(Enumerable.Repeat(chars, 7)
              .Select(s => s[random.Next(s.Length)]).ToArray());
        }
    }
}