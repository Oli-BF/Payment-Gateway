using System;

namespace PG_Core.Services.Bank.Models
{
    /// <summary>
    /// Payment Response model to encapsulate the payment response to the payment gateway.
    /// </summary>
    public class ToPgPayResp
    {
        public Guid paymentId { get; set; }

        public bool paymentSuccessful { get; set; }
    }
}