using PG_Core.Services.Bank;
using PG_Core.Services.Bank.Models;
using System;
using System.Threading.Tasks;

namespace PG_Tests.Bank
{
    /// <summary>
    /// The MockBankFailure class takes in the payment request from the payment gateway (Payment 
    /// Controller Class) and returns a Bank payment response as an ToPgPayResp object.
    /// </summary>
    class MockBankFailure : IMockBank
    {
        /// <summary>
        /// A known Guid is returned each time, as well as a bool indicating a failed payment.
        /// </summary>
        /// <param name="bankPaymentRequest"></param>
        /// <returns> Bank payment response as an ToPgPayResp object </returns>
        public async Task<ToPgPayResp> ValidatePaymentAsync(FromPgPayReq bankPaymentRequest)
        {
            var bankPaymentResponse = new ToPgPayResp()
            {
                paymentId = Guid.Parse("530fa534-c2a4-41f9-9b20-f83368ad351b"),
                paymentSuccessful = false
            };
            return await Task.FromResult(bankPaymentResponse);
        }
    }
}