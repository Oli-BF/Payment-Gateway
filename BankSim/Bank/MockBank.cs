using BankSim.Models;
using System;
using System.Threading.Tasks;

namespace BankSim.Bank
{
    public class MockBank : IMockBank
    {
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