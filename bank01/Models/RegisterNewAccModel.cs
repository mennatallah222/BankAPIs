using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace bank01.Models
{
    public class RegisterNewAccModel
    {
        [Key]
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        //public string AccountName { get; set; }
        public string PhoneNumber { get; set; }
        public string Email { get; set; }
        //public decimal CurrentBalance { get; set; }
        public AccountType AccountType { get; set; }
        //public byte[] PinHash { get; set; }
        //public byte[] PinSalt { get; set; }
        public DateTime DateCreated { get; set; }
        public DateTime DateLastUpdated { get; set; }

        [Required]
        [RegularExpression(@"^\d{1,4}$", ErrorMessage = "Pin must be between 1 and 4 digits")]
        public string Pin { get; set; }
        [Required]
        [Compare("Pin", ErrorMessage ="Pins don't match")]
        public string ConfirmPin { get; set; }
    }
}
