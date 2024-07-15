using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace bank01.Models
{
    [Table("Transactions")]
    public class Transaction
    {
        [Key]
        public int Id { get; set; }
        public string TUniqueReference { get; set; }
        public decimal TAmount { get; set; }
        public TransactionStatus TStatus { get; set; }
        public bool IsSuccessful => TStatus.Equals(TransactionStatus.Success);
        public string TSourceAcc { get; set; }
        public string TDestAcc { get; set; }
        public string TParticulars { get; set; }
        public Ttype TransactionType { get; set; }
        public DateTime TDate { get; set; }

        public Transaction()
        {
            TUniqueReference = $"{Guid.NewGuid().ToString().Replace("-", "").Substring(1, 27)}";
        }
    }


    public enum TransactionStatus
    {
        Failed, Success, Error
    }

    public enum Ttype
    {
        Deposit, Withdrawal, Transfer
    }
}
