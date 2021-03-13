using PG_Core.Services.Bank.Models;
using System.Threading.Tasks;

namespace PG_Core.Services.Bank
{
    public interface IMockBank
    {
        public Task<ToPgPayResp> ValidatePaymentAsync(FromPgPayReq bankPaymentRequest);
    }
}