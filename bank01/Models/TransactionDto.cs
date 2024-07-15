using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace bank01.Models
{
    public class TransactionDto
    {
        public decimal TAmount { get; set; }
        public string TSourceAcc { get; set; }
        public string TDestAcc { get; set; }
        public Ttype TransactionType { get; set; }
        public DateTime TDate { get; set; }
    }
}
