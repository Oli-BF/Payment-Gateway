using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

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