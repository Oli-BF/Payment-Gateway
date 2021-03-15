using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.RegularExpressions;

namespace PG_DataAccess.Models
{
    /// <summary>
    /// Payment Request model to encapsulate the payment request, as well as to create the migrations 
    /// for the database. The database annotations are provided here also. Furthemore, it is here
    /// where the card number inputted by the merchant is masked before being stored in the database
    /// and subsequently returned to the merchant upon request.
    /// </summary>
    public class PaymentRequest
    {
        [Column("id")]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        [Key]
        public Guid paymentId { get; set; }

        [Column("currency")]
        [Required(ErrorMessage = "{0} is Required")]
        [DataType(DataType.Currency)]
        public string currency { get; set; }

        [Column("amount")]
        [Required(ErrorMessage = "{0} is Required")]
        public decimal amount { get; set; }

        private string cardNumber;

        [Encrypted]
        [Column("card_number", TypeName = "varchar(100)")]
        [Required(ErrorMessage = "{0} is Required")]
        public string cardNumberMasked
        {
            get => cardNumber = Regex.Replace(cardNumber, "[0-9](?=[0-9]{4})", "*");
            set => cardNumber = value;
        }

        [Encrypted]
        [Column("card_expiry_date", TypeName = "varchar(100)")]
        [Required(ErrorMessage = "{0} is Required")]
        public string expiryDate { get; set; }

        [Encrypted]
        [Column("card_holder", TypeName = "varchar(100)")]
        [Required(ErrorMessage = "{0} is Required")]
        public string cardHolder { get; set; }

        [Column("payment_successful")]
        [Required(ErrorMessage = "{0} is Required")]
        public bool paymentSuccessful { get; set; }

        [Column("date_created")]
        [DataType(DataType.DateTime)]
        public DateTime dateCreated { get; set; } = DateTime.UtcNow;
    }
}