using BankSim.Models;
using System.Threading.Tasks;

namespace BankSim.Bank
{
    public interface IMockBank
    {
        public Task<ToPgPayResp> ValidatePaymentAsync(FromPgPayReq bankPaymentRequest);
    }
}