using AutoMapper;
using bank01.DAL;
using bank01.Models;
using bank01.Services.Interfaces;
using bank01.Utils;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace bank01.Services.Implementations
{
    public class TransactionService : ITransaction
    {
        private BankDbContext _bankDbContext;
        ILogger<ITransaction> _logger;
        private AppSetting _setting;
        private static string _BankSettlementAccount;
        private readonly IAccount _accountService;
        private readonly IMapper _mapper;

        public TransactionService(BankDbContext bankDbContext, ILogger<ITransaction> logger, IOptions<AppSetting> settings,
            IAccount account, IMapper mapper)
        {
            _bankDbContext = bankDbContext;
            _logger = logger;
            _setting = settings.Value;
            _BankSettlementAccount = _setting.BankSettlementAccount;
            _accountService = account;
            _mapper = mapper;
        }
        public Response CreateNewTransaction(Transaction transaction)
        {
            Response response = new Response();
            _bankDbContext.Transactions.Add(transaction);
            _bankDbContext.SaveChanges();
            response.ResponseCode = "00";
            response.ResponseMessage = "Transaction created successfully";
            response.Data = null;
            return response;
        }

        public Response FindTransactionByDate(DateTime dateTime)
        {
            var transaction = _bankDbContext.Transactions.Where(x => x.TDate == dateTime).ToList();
            Response response = new Response();
            response.ResponseCode = "00";
            response.ResponseMessage = "Transaction created successfully";
            response.Data = transaction;
            return response;
        }

        public Response MakeDeposit(string AccNum, decimal Amount, string TPin)
        {
            Response response = new Response();
            Account srcAcc;
            Account destAcc;
            Transaction transaction = new Transaction();

            var authenticatedUser = _accountService.Aunthenticate(AccNum, TPin);
            if (authenticatedUser == null) throw new ApplicationException("Invalid Credintials");

            try
            {
                //for deposit, the bank settlement account is the source giving money to the user's account
                srcAcc = _accountService.GetAccountNum(_BankSettlementAccount);
                destAcc = _accountService.GetAccountNum(AccNum);

                //update the accounts balances
                srcAcc.CurrentBalance -= Amount;
                destAcc.CurrentBalance += Amount;

                //check if there's an update happened ====> EntityState.Modified
                if ((_bankDbContext.Entry(srcAcc).State==Microsoft.EntityFrameworkCore.EntityState.Modified 
                    && (_bankDbContext.Entry(destAcc).State == Microsoft.EntityFrameworkCore.EntityState.Modified)))
                {
                    transaction.TStatus = TransactionStatus.Success;
                    response.ResponseCode = "00";
                    response.ResponseMessage = "Transaction successful";
                    response.Data = $"Updated balance of your account is: {srcAcc.CurrentBalance}";
                }
                else
                {
                    transaction.TStatus = TransactionStatus.Failed;
                    response.ResponseCode = "02";
                    response.ResponseMessage = "Transaction failed";
                    response.Data = null;
                }

            }
            catch(Exception e)
            {
                _logger.LogError($"AN ERROR OCCURED =>{e.Message}");
            }

            transaction.TransactionType = Ttype.Deposit;
            transaction.TSourceAcc = _BankSettlementAccount;
            transaction.TDestAcc = AccNum;
            transaction.TAmount = Amount;
            transaction.TDate = DateTime.Now;
            transaction.TParticulars = $"NEW TRANSACTION FROM SOURCE {JsonConvert.SerializeObject(transaction.TSourceAcc)} TO DEST ACCOUNT {JsonConvert.SerializeObject(transaction.TDestAcc)} " +
                $"ON DATE {transaction.TDate} FOR AMOUNT {transaction.TAmount} TRANSACTION TYPE: {transaction.TransactionType}" +
                $"TRASNSACTION STATUS: {transaction.TStatus}";

            _bankDbContext.Transactions.Add(transaction);
            _bankDbContext.SaveChanges();

            return response;
        }

        public Response MakeFundsTransfer(string FromAcc, string ToAcc, decimal Amount, string TPin)
        {
            Response response = new Response();
            Account srcAcc;
            Account destAcc;
            Transaction transaction = new Transaction();

            var authenticatedUser = _accountService.Aunthenticate(FromAcc, TPin);
            if (authenticatedUser == null) throw new ApplicationException("Invalid Credintials");

            try
            {
                //for deposit, the bank settlement account is the source giving money to the user's account
                srcAcc = _accountService.GetAccountNum(FromAcc);
                destAcc = _accountService.GetAccountNum(ToAcc);

                //update the accounts balances
                srcAcc.CurrentBalance -= Amount;
                destAcc.CurrentBalance += Amount;

                //check if there's an update happened ====> EntityState.Modified
                if ((_bankDbContext.Entry(srcAcc).State == Microsoft.EntityFrameworkCore.EntityState.Modified
                    && (_bankDbContext.Entry(destAcc).State == Microsoft.EntityFrameworkCore.EntityState.Modified)))
                {
                    transaction.TStatus = TransactionStatus.Success;
                    response.ResponseCode = "00";
                    response.ResponseMessage = "Transaction successful";
                    response.Data = $"Updated balance of your account is: {srcAcc.CurrentBalance}";
                }
                else
                {
                    transaction.TStatus = TransactionStatus.Failed;
                    response.ResponseCode = "02";
                    response.ResponseMessage = "Transaction failed";
                    response.Data = null;
                }

            }
            catch (Exception e)
            {
                _logger.LogError($"AN ERROR OCCURED =>{e.Message}");
            }

            transaction.TransactionType = Ttype.Transfer;
            transaction.TSourceAcc = FromAcc;
            transaction.TDestAcc = ToAcc;
            transaction.TAmount = Amount;
            transaction.TDate = DateTime.Now;
            transaction.TParticulars = $"NEW TRANSACTION FROM SOURCE {JsonConvert.SerializeObject(transaction.TSourceAcc)} TO DEST ACCOUNT {JsonConvert.SerializeObject(transaction.TDestAcc)} " +
                $"ON DATE {transaction.TDate} FOR AMOUNT {transaction.TAmount} TRANSACTION TYPE: {transaction.TransactionType}" +
                $"TRASNSACTION STATUS: {transaction.TStatus}";

            _bankDbContext.Transactions.Add(transaction);
            _bankDbContext.SaveChanges();

            return response;
        }

        public Response MakeWithdrawal(string AccNum, decimal Amount, string TPin)
        {
            Response response = new Response();
            Account srcAcc;
            Account destAcc;
            Transaction transaction = new Transaction();

            var authenticatedUser = _accountService.Aunthenticate(AccNum, TPin);
            if (authenticatedUser == null) throw new ApplicationException("Invalid Credintials");

            try
            {
                //for deposit, the bank settlement account is the source giving money to the user's account
                srcAcc = _accountService.GetAccountNum(AccNum);
                destAcc = _accountService.GetAccountNum(_BankSettlementAccount);

                //update the accounts balances
                srcAcc.CurrentBalance -= Amount;
                destAcc.CurrentBalance += Amount;

                //check if there's an update happened ====> EntityState.Modified
                if ((_bankDbContext.Entry(srcAcc).State == Microsoft.EntityFrameworkCore.EntityState.Modified
                    && (_bankDbContext.Entry(destAcc).State == Microsoft.EntityFrameworkCore.EntityState.Modified)))
                {
                    transaction.TStatus = TransactionStatus.Success;
                    response.ResponseCode = "00";
                    response.ResponseMessage = "Transaction successful";

                    //for real-time balance updates
                    response.Data = $"Updated balance of your account is: {srcAcc.CurrentBalance}";
                }
                else
                {
                    transaction.TStatus = TransactionStatus.Failed;
                    response.ResponseCode = "02";
                    response.ResponseMessage = "Transaction failed";
                    response.Data = null;
                }

            }
            catch (Exception e)
            {
                _logger.LogError($"AN ERROR OCCURED =>{e.Message}");
            }

            transaction.TransactionType = Ttype.Withdrawal;
            transaction.TSourceAcc = AccNum;
            transaction.TDestAcc = _BankSettlementAccount;
            transaction.TAmount = Amount;
            transaction.TDate = DateTime.Now;
            transaction.TParticulars = $"NEW TRANSACTION FROM SOURCE {JsonConvert.SerializeObject(transaction.TSourceAcc)} TO DEST ACCOUNT {JsonConvert.SerializeObject(transaction.TDestAcc)} " +
                $"ON DATE {transaction.TDate} FOR AMOUNT {transaction.TAmount} TRANSACTION TYPE: {transaction.TransactionType}" +
                $"TRASNSACTION STATUS: {transaction.TStatus}";

            _bankDbContext.Transactions.Add(transaction);
            _bankDbContext.SaveChanges();

            return response;
        }

        
        List<Transaction> ITransaction.GetTransactionHistory(string AccNum)
        {
            var t = _bankDbContext.Transactions.Where(t => t.TSourceAcc == AccNum).ToList();
            return t;
        }
    }
}
