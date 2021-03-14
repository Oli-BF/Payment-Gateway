using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace PG_Core.Models
{
    public class ToMerchPayResp
    {
        public int paymentId { get; set; }

        public string currency { get; set; }

        public decimal amount { get; set; }

        public string cardNumberMasked { get; set; }

        public string expiryDate { get; set; }

        public string cardHolder { get; set; }

        public bool paymentSuccessful { get; set; }

        public DateTime dateCreated { get; set; }
    }
}