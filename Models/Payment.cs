using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;

namespace MVC_project.Models
{
    public class Payment
    {
        [StringLength(16, MinimumLength = 16, ErrorMessage = "Credit Card need to be 16 characters")]
        [RegularExpression("^([0-9 .&'-]+)$", ErrorMessage = "Invalid Credit Card")]
        [Required(ErrorMessage = "Credit Card is required")]
        [DisplayName("Credit Card:")]

        public string CardNumber { get; set; }

        [Required(ErrorMessage = "Expiration Date not in format MM/YY")]
        public string ExpirationYear { get; set; }

        [Required(ErrorMessage = "Expiration Date not in format MM/YY")]
        public string ExpirationMonth { get; set; }

        [Required(ErrorMessage = "Expiration Date is required")]
        [StringLength(5, MinimumLength = 5, ErrorMessage = "Expiration Date need to be in format MM/YY")]
        [RegularExpression("^([0-9/ .&'-]+)$", ErrorMessage = "Expiration Date need to be in format MM/YY")]
        [DisplayName("Expiration Date:")]
        public string ExpirationDate
        {
            get
            {
                return string.Format("MM{0}/YY{1}", ExpirationMonth, ExpirationYear);
            }
            set { }
        }

        [Required(ErrorMessage = "CVC is required")]
        [RegularExpression("^([0-9 .&'-]+)$", ErrorMessage = "Invalid CVC")]
        [StringLength(3, MinimumLength = 3, ErrorMessage = "CVC need to be 3 characters")]
        [DisplayName("CVC:")]
        public string CVC { get; set; }
    }
}