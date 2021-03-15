using System;

namespace PG_Core.Models
{
    /// <summary>
    /// Bank Response model to encapsulate the banks's response from the payment request.
    /// </summary>
    public class FromBankPayResp
    {
        public Guid paymentId { get; set; }

        public bool paymentSuccessful { get; set; }
    }
}