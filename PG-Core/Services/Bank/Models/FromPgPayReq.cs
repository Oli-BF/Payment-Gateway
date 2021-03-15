
namespace PG_Core.Services.Bank.Models
{
    /// <summary>
    /// Payment Request model to encapsulate the payment request from the payment gateway.
    /// </summary>
    public class FromPgPayReq
    {
        public string currency { get; set; }

        public decimal amount { get; set; }

        public string cardNumber { get; set; }

        public string expiryDate { get; set; }

        public int cvv { get; set; }

        public string cardHolder { get; set; }
    }
}