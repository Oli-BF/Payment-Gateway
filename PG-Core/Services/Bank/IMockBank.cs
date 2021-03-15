using PG_Core.Services.Bank.Models;
using System.Threading.Tasks;

namespace PG_Core.Services.Bank
{
    /// <summary>
    /// Interface for the MockBank.
    /// </summary>
    public interface IMockBank
    {
        public Task<ToPgPayResp> ValidatePaymentAsync(FromPgPayReq bankPaymentRequest);
    }
}