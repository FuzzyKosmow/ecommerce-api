using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ecommerce_api.DTO.Payment;
using ecommerce_api.Services.PaymentService;
using Microsoft.AspNetCore.Mvc;

namespace ecommerce_api.Controllers
{
    /// <summary>
    /// Controller for handling payment. Current implementation is for charging credit card only and is placeholder
    /// </summary>
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
        /// <summary>
        /// Charge a credit card with the given information
        /// </summary>
        /// <param name="chargeCreditCard">
        ///     CardNumber: The credit card number
        ///     CardHolder: The name of the card holder
        ///     CardExpireDate: The expire date of the card
        ///     CardCvv: The CVV of the card
        ///     Amount: The amount to charge
        /// </param>
        /// <returns>
        ///     200: The payment result
        ///     400: Bad request if the card is invalid
        ///     500: Internal server error if the payment service is down
        /// </returns>
        [HttpPost]
        public async Task<IActionResult> ChargeCreditCard([FromBody] PaymentChargeDTO chargeCreditCard)
        {
            var result = await _paymentService.ChargeCreditCard(chargeCreditCard.CardNumber, chargeCreditCard.CardHolder, chargeCreditCard.CardExpireDate, chargeCreditCard.CardCvv, chargeCreditCard.Amount);
            return Ok(result);
        }
    }
}