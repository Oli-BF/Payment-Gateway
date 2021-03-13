using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace PG_Core.Models
{
    public class FromMerchPayReq
    {
        [Required(ErrorMessage = "{0} is Required")]
        [DataType(DataType.Currency)]
        public string currency { get; set; }

        [Required(ErrorMessage = "{0} is Required")]
        [Range(typeof(decimal), "0", "79228162514264337593543950335", 
                                ErrorMessage = "Only positive values are allowed")]
        public decimal amount { get; set; }

        [Required(ErrorMessage = "{0} is Required")]
        [MaxLength(19, ErrorMessage = "Card Number should not exceed 19 digits in length")]
        [MinLength(13, ErrorMessage = "Card Number should be no less than 13 digits in length")]
        public string cardNumber { get; set; }

        [Required(ErrorMessage = "{0} is Required")]
        [MaxLength(20, ErrorMessage = "Expiry Date should not exceed 20 digits/characters in length")]
        public string expiryDate { get; set; }

        [Required(ErrorMessage = "{0} is Required")]
        [Range(111,9999, ErrorMessage = "CVV must be between 111 and 9999")]
        public int cvv { get; set; }

        [Required(ErrorMessage = "{0} is Required")]
        [MaxLength(100, ErrorMessage = "Card Holder's Name should not exceed 100 characters in length")]
        public string cardHolder { get; set; }
    }
}