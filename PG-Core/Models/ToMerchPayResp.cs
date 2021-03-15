using System;

namespace PG_Core.Models
{
    /// <summary>
    /// Merchant Payment Response model to encapsulate the details returned when requested by the merchant.
    /// </summary>
    public class ToMerchPayResp
    {
        public Guid paymentId { get; set; }

        public string currency { get; set; }

        public decimal amount { get; set; }

        public string cardNumberMasked { get; set; }

        public string expiryDate { get; set; }

        public string cardHolder { get; set; }

        public bool paymentSuccessful { get; set; }

        public DateTime dateCreated { get; set; }
    }
}