using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PG_Core.Models
{
    public class FromBankPayResp
    {
        public int paymentId { get; set; }

        public bool paymentSuccessful { get; set; }
    }
}