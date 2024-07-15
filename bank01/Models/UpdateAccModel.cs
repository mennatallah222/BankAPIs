using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace bank01.Models
{
    public class UpdateAccModel
    {
        [Key]
        public int Id { get; set; }
        public string PhoneNumber { get; set; }
        public string Email { get; set; }
        [Required]
        [RegularExpression(@"^[0-9]{4}$", ErrorMessage = "Pin must be no more than 4 digits")]
        public string Pin { get; set; }
        [Compare("Pin", ErrorMessage = "Pins don't match")]
        public string ConfirmPin { get; set; }

        public DateTime DateLastUpdated { get; set; }
    }
}
