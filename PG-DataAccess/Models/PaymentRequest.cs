using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PG_DataAccess.Models
{
    public class PaymentRequest
    {
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        [Key]
        public int paymentId { get; set; }

        [Required(ErrorMessage = "{0} is Required")]
        [DataType(DataType.Currency)]
        public string currency { get; set; }

        [Required(ErrorMessage = "{0} is Required")]
        public decimal amount { get; set; }

        [Encrypted]
        [Column(TypeName = "varchar(100)")]
        [Required(ErrorMessage = "{0} is Required")]
        public string cardNumberMasked { get; set; }

        [Encrypted]
        [Column(TypeName = "varchar(100)")]
        [Required(ErrorMessage = "{0} is Required")]
        public string expiryDate { get; set; }

        [Encrypted]
        [Column(TypeName = "varchar(100)")]
        [Required(ErrorMessage = "{0} is Required")]
        public string cardHolder { get; set; }

        [Required(ErrorMessage = "{0} is Required")]
        public bool paymentSuccessful { get; set; }

        [DataType(DataType.DateTime)]
        public DateTime dateCreated { get; set; } = DateTime.UtcNow;
    }
}