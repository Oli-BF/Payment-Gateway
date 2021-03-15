using PG_Core.Services.Bank;
using PG_Core.Services.Bank.Models;
using System;
using System.Threading.Tasks;

namespace PG_Tests.Bank
{
    /// <summary>
    /// The MockBankSuccess class takes in the payment request from the payment gateway (Payment 
    /// Controller Class) and returns a Bank payment response as an ToPgPayResp object.
    /// </summary>
    class MockBankSuccess : IMockBank
    {
        /// <summary>
        /// A known Guid is returned each time, as well as a bool indicating a successful payment.
        /// </summary>
        /// <param name="bankPaymentRequest"></param>
        /// <returns> Bank payment response as an ToPgPayResp object </returns>
        public async Task<ToPgPayResp> ValidatePaymentAsync(FromPgPayReq bankPaymentRequest)
        {
            var bankPaymentResponse = new ToPgPayResp()
            {
                paymentId = Guid.Parse("e191c52e-d815-4ad4-8982-45b64d138fd6"),
                paymentSuccessful = true
            };
            return await Task.FromResult(bankPaymentResponse);
        }
    }
}