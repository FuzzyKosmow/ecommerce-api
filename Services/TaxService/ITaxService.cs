using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ecommerce_api.Services.TaxService
{
    public interface ITaxService
    {

        public Task<decimal> CalculateTax(decimal subTotal);
    }
}