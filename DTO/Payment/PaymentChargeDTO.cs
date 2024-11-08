using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ecommerce_api.DTO.Payment
{
    public class PaymentChargeDTO
    {

        public string CardNumber { get; set; }
        public string CardHolder { get; set; }
        public string CardExpireDate { get; set; }
        public string CardCvv { get; set; }
        public decimal Amount { get; set; }
    }
}