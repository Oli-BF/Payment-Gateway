using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using PG_Core.Models;
using PG_Core.Services.Bank;
using PG_Core.Services.Bank.Models;
using PG_DataAccess.Data;
using PG_DataAccess.Models;
using System;
using System.Threading.Tasks;

namespace PG_Core.Controllers
{
    /// <summary>
    /// This Class is the Payment Controller.
    /// </summary>
    [Route("payment")]
    [ApiController]
    [Authorize]
    public class PaymentController : ControllerBase
    {
        // Field Variables
        private readonly PgDbContext pgDbContext;
        private readonly IMockBank aquiringBank;
        private readonly ILogger<PaymentController> iLogger;

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="dbContext"></param>
        /// <param name="mockBank"></param>
        /// <param name="logger"></param>
        public PaymentController(PgDbContext dbContext, IMockBank mockBank, ILogger<PaymentController> logger)
        {
            pgDbContext = dbContext;
            aquiringBank = mockBank;
            iLogger = logger;
        }

        /// <summary>
        /// GetPaymentByIdAsync is the GET request for merchants requesting payment details of a
        /// particular payment.
        /// </summary>
        /// <param name="paymentId"></param>
        /// <returns> Merchant Payment Response as a paymentRequestan object. </returns>
        [HttpGet("{paymentId}")]
        public async Task<ActionResult<ToMerchPayResp>> GetPaymentByIdAsync(Guid paymentId)
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

        /// <summary>
        /// Helper method for GetPaymentByIdAsync.
        /// </summary>
        /// <param name="paymentRequest"></param>
        /// <returns> A merchant payment request as a ToMerchPayResp object. </returns>
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

        /// <summary>
        /// PostPaymentRequestAsync is the POST request of merchants wishing to process a
        /// payment through the payment gateway.
        /// </summary>
        /// <param name="paymentRequest"></param>
        /// <returns> Bank Payment Response as a FromBankPayResp object. </returns>
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

        /// <summary>
        /// Helper method for PostPaymentRequestAsync.
        /// </summary>
        /// <param name="fromMerchPayReq"></param>
        /// <returns> Payment gateway payment request as a FromPgPayReq object. </returns>
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

        /// <summary>
        /// Helper method for PostPaymentRequestAsync.
        /// </summary>
        /// <param name="toPgPayResp"></param>
        /// <param name="fromMerchPayReq"></param>
        /// <returns> Payment Request as a PaymentRequest object. </returns>
        private static PaymentRequest DataPaymentRequest(
            ToPgPayResp toPgPayResp, FromMerchPayReq fromMerchPayReq) => 
            new PaymentRequest
            {
                paymentId = toPgPayResp.paymentId,
                currency = fromMerchPayReq.currency,
                amount = fromMerchPayReq.amount,
                cardNumberMasked = fromMerchPayReq.cardNumber,
                expiryDate = fromMerchPayReq.expiryDate,
                cardHolder = fromMerchPayReq.cardHolder,
                paymentSuccessful = toPgPayResp.paymentSuccessful
            };

        /// <summary>
        /// Helper method for PostPaymentRequestAsync.
        /// </summary>
        /// <param name="toPgPayResp"></param>
        /// <returns> Bank Payment Response as a FromBankPayResp object. </returns>
        private static FromBankPayResp BankPaymentResponse(ToPgPayResp toPgPayResp) =>
            new FromBankPayResp
            {
                paymentId = toPgPayResp.paymentId,
                paymentSuccessful = toPgPayResp.paymentSuccessful
            };
    }
}