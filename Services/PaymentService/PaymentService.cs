using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ecommerce_api.Services.PaymentService
{
    public class PaymentService : IPaymentService
    {
        public async Task<bool> ChargeCreditCard(string cardNumber, string cardHolder, string cardExpireDate, string cardCvv, decimal amount)
        {
            return true;
        }
    }
}