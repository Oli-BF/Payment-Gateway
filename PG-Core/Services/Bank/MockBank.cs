using PG_Core.Services.Bank.Models;
using System;
using System.Threading.Tasks;

namespace PG_Core.Services.Bank
{
    /// <summary>
    /// The MockBank class takes in the payment request from the payment gateway (Payment Controller Class)
    /// and returns a Bank payment response as an ToPgPayResp object.
    /// </summary>
    public class MockBank : IMockBank
    {
        /// <summary>
        /// A new Guid is returned each time, as well as a random chance for the payment to be successful,
        /// to simulate real requests.
        /// </summary>
        /// <param name="bankPaymentRequest"></param>
        /// <returns> Bank payment response as an ToPgPayResp object </returns>
        public async Task<ToPgPayResp> ValidatePaymentAsync(FromPgPayReq bankPaymentRequest)
        {
            var rnd = new Random();
            var bankPaymentResponse = new ToPgPayResp();

            var randomCase = rnd.Next(1, 3);

            switch(randomCase)
            {
                case 1:
                    bankPaymentResponse = new ToPgPayResp()
                    {
                        paymentId = Guid.NewGuid(),
                        paymentSuccessful = true
                    };
                    break;
                case 2:
                    bankPaymentResponse = new ToPgPayResp()
                    {
                        paymentId = Guid.NewGuid(),
                        paymentSuccessful = false
                    };
                    break;
            }
            return await Task.FromResult(bankPaymentResponse);
        }
    }
}