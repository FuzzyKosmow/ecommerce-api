using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ecommerce_api.Services.PaymentService
{
    public interface IPaymentService
    {
        Task<bool> ChargeCreditCard(string cardNumber, string cardHolder, string cardExpireDate, string cardCvv, decimal amount);

    }
}