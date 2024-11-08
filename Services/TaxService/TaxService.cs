using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ecommerce_api.Services.TaxService
{
    public class TaxService : ITaxService
    {
        private readonly decimal taxRate = 0.1m;

        public async Task<decimal> CalculateTax(decimal subTotal)
        {
            return subTotal * taxRate;
        }

    }
}