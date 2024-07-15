using bank01.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace bank01.Services
{
    public interface IAccount
    {
        Account Aunthenticate(string AccNum, string Pin);
        IEnumerable<Account> GetAllAccounts();
        Account Create(Account account, string Pin, string ConfirmPin);
        void Update(Account account, string Pin = null);
        void Delete(int id);
        Account GetById(int id);
        Account GetAccountNum(string AccountNum);
        decimal GetBalance(string AccountNum);
    }
}
