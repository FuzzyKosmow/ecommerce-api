using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ecommerce_api.DTO.Payment;
using ecommerce_api.Services.PaymentService;
using Microsoft.AspNetCore.Mvc;

namespace ecommerce_api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PaymentController : ControllerBase
    {
        // Allow for charge credit card
        private readonly IPaymentService _paymentService;

        public PaymentController(IPaymentService paymentService)
        {
            _paymentService = paymentService;
        }

        [HttpPost]
        public async Task<IActionResult> ChargeCreditCard([FromBody] PaymentChargeDTO chargeCreditCard)
        {
            var result = await _paymentService.ChargeCreditCard(chargeCreditCard.CardNumber, chargeCreditCard.CardHolder, chargeCreditCard.CardExpireDate, chargeCreditCard.CardCvv, chargeCreditCard.Amount);
            return Ok(result);
        }
    }
}