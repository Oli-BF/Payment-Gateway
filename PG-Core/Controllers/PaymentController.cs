using BankSim.Bank;
using BankSim.Models;
using Microsoft.AspNetCore.Mvc;
using PG_Core.Models;
using PG_DataAccess.Data;
using PG_DataAccess.Models;
using System;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace PG_Core.Controllers
{
    [ApiController]
    [Route("payments")]
    public class PaymentController : ControllerBase
    {
        private readonly PgDbContext pgDbContext;
        private readonly IMockBank aquiringBank;

        public PaymentController(PgDbContext dbContext, IMockBank mockBank)
        {
            pgDbContext = dbContext;
            aquiringBank = mockBank;
        }

        [HttpGet("{paymentId}")]
        public async Task<ActionResult<ToMerchPayResp>> GetPaymentByIdAsync(Guid paymentId)
        {
            var paymentRequest = await pgDbContext.paymentRequests.FindAsync(paymentId);

            if (paymentRequest == null)
            {
                return NotFound();
            }

            return MerchPaymentResponse(paymentRequest);
        }

        [HttpPost]
        public async Task<ActionResult<FromBankPayResp>> PostPaymentRequestAsync(FromMerchPayReq paymentRequest)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            // Get Response from bank
            var bankPaymentResponse = aquiringBank.ValidatePaymentAsync(
                BankPaymentRequest(paymentRequest)).Result;

            // Add Payment Request to Database
            var payment = DataPaymentRequest(bankPaymentResponse, paymentRequest);
            pgDbContext.paymentRequests.Add(payment);
            await pgDbContext.SaveChangesAsync();

            // Payment Response from the Bank, to the Payment Gateway, to the Merchant
            return BankPaymentResponse(bankPaymentResponse);
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