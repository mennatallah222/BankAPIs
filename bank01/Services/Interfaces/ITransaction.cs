using bank01.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Transactions;

namespace bank01.Services.Interfaces
{
    public interface ITransaction
    {
        Response CreateNewTransaction(Models.Transaction transaction);
        Response FindTransactionByDate(DateTime dateTime);
        Response MakeDeposit(string AccNum, decimal Amount, string TPin);
        Response MakeWithdrawal(string AccNum, decimal Amount, string TPin);
        Response MakeFundsTransfer(string FromAcc, string ToAcc, decimal Amount, string TPin);
        List<Models.Transaction> GetTransactionHistory(string AccNum);
    }
}
