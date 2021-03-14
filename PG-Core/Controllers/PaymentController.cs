using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using PG_Core.Models;
using PG_Core.Services.Bank;
using PG_Core.Services.Bank.Models;
using PG_DataAccess.Data;
using PG_DataAccess.Models;
using System;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

/*
 * One note about setting up your controllers this way: There is intense debate about whether you should inject a DbContext into your controllers. Many people believe you should abstract the data layer away from the controllers and use models and service classes here rather than entities and contexts. I tend to agree with that philosophy, however there is always a “but.” For this tutorial, it would be okay to inject the context into the controller, however this strategy may fail you one day and you have to write models anyway.
 */
namespace PG_Core.Controllers
{
    [Route("payment")]
    [ApiController]
    [Authorize]
    public class PaymentController : ControllerBase
    {
        private readonly PgDbContext pgDbContext;
        private readonly IMockBank aquiringBank;
        private readonly ILogger<PaymentController> iLogger;

        public PaymentController(PgDbContext dbContext, IMockBank mockBank, ILogger<PaymentController> logger)
        {
            pgDbContext = dbContext;
            aquiringBank = mockBank;
            iLogger = logger;
        }


        [HttpGet("{paymentId}")]
        public async Task<ActionResult<ToMerchPayResp>> GetPaymentByIdAsync(int paymentId)
        {
            iLogger.LogInformation("Called Get Payment by ID.");

            var paymentRequest = await pgDbContext.paymentRequests.FindAsync(paymentId);

            if (paymentRequest == null)
            {
                iLogger.LogWarning("Payment Request is null, Payment ID cannot be found.");
                return NotFound();
            }

            return MerchPaymentResponse(paymentRequest);
        }

        private static ToMerchPayResp MerchPaymentResponse(PaymentRequest paymentRequest) =>
            new ToMerchPayResp
            {
                paymentId = paymentRequest.paymentId,
                currency = paymentRequest.currency,
                amount = paymentRequest.amount,
                cardNumberMasked = paymentRequest.cardNumberMasked,
                expiryDate = paymentRequest.expiryDate,
                cardHolder = paymentRequest.cardHolder,
                paymentSuccessful = paymentRequest.paymentSuccessful,
                dateCreated = paymentRequest.dateCreated,
            };


        [HttpPost]
        public async Task<ActionResult<FromBankPayResp>> PostPaymentRequestAsync(FromMerchPayReq paymentRequest)
        {
            iLogger.LogInformation("Called Post Payment Request.");

            if (!ModelState.IsValid)
            {
                iLogger.LogWarning("Model state is not valid.");
                return BadRequest(ModelState);
            }

            // Get Response from bank
            var bankPaymentResponse = aquiringBank.ValidatePaymentAsync(
                BankPaymentRequest(paymentRequest)).Result;

            if (bankPaymentResponse == null)
            {
                iLogger.LogWarning("Bank Payment Response is null.");
                return NotFound();
            }

            // Add Payment Request to Database
            var payment = DataPaymentRequest(bankPaymentResponse, paymentRequest);
            pgDbContext.paymentRequests.Add(payment);
            await pgDbContext.SaveChangesAsync();

            iLogger.LogInformation("Payment Request saved to Database.");

            // Payment Response from the Bank, to the Payment Gateway, to the Merchant
            return BankPaymentResponse(bankPaymentResponse);
        }

        private static FromPgPayReq BankPaymentRequest(FromMerchPayReq fromMerchPayReq) => 
            new FromPgPayReq
            {
                currency = fromMerchPayReq.currency,
                amount = fromMerchPayReq.amount,
                cardNumber = fromMerchPayReq.cardNumber,
                expiryDate = fromMerchPayReq.expiryDate,
                cvv = fromMerchPayReq.cvv,
                cardHolder = fromMerchPayReq.cardHolder
            };

        private static PaymentRequest DataPaymentRequest(
            ToPgPayResp toPgPayResp, FromMerchPayReq fromMerchPayReq) => 
            new PaymentRequest
            {
                paymentId = toPgPayResp.paymentId,
                currency = fromMerchPayReq.currency,
                amount = fromMerchPayReq.amount,
                // https://stackoverflow.com/questions/54813746/how-to-mask-first-6-and-last-4-digits-for-a-credit-card-number-in-net - (?<=\d{4}[ -]?\d{2})\d{2}[ -]?\d{4}
                cardNumberMasked = Regex.Replace(fromMerchPayReq.cardNumber.ToString(), 
                                                 "[0-9](?=[0-9]{4})", "*"),
                expiryDate = fromMerchPayReq.expiryDate,
                cardHolder = fromMerchPayReq.cardHolder,
                paymentSuccessful = toPgPayResp.paymentSuccessful
            };

        private static FromBankPayResp BankPaymentResponse(ToPgPayResp toPgPayResp) =>
            new FromBankPayResp
            {
                paymentId = toPgPayResp.paymentId,
                paymentSuccessful = toPgPayResp.paymentSuccessful
            };
    }
}