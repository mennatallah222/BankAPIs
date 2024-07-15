using AutoMapper;
using bank01.Models;
using bank01.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace bank01.Controllers
{
    [ApiController]
    [Route("api/v3/[controller]")]
    public class TransactionController : ControllerBase
    {
        private ITransaction _transactionService;
        IMapper _mapper;
        public TransactionController(ITransaction transactionService, IMapper mapper)
        {
            _transactionService = transactionService;
            _mapper = mapper;
        }

        [HttpPost]
        [Route("create_new_transaction")]
        public IActionResult CreateNewTransaction([FromBody] TransactionDto transaction)
        {
            //made a dto bec the transaction model has elements that the user won't fill

            if (!ModelState.IsValid) return BadRequest(transaction);

            var t = _mapper.Map<Transaction>(transaction);
            return Ok(_transactionService.CreateNewTransaction(t));
        }

        [HttpPost]
        [Route("make_deposit")]
        public IActionResult MakeDeposit(string AccNum, decimal Amount, string TPin)
        {
            if (!Regex.IsMatch(AccNum, @"^[0][1-9]\d{9}$|^[1-9]\d{9}$")) return BadRequest("Account num must be 10 digits");
            return Ok(_transactionService.MakeDeposit(AccNum, Amount, TPin));
        }

        [HttpPost]
        [Route("make_withdrawal")]
        public IActionResult MakeWithdrawal(string AccNum, decimal Amount, string TPin)
        {
            if (!Regex.IsMatch(AccNum, @"^[0][1-9]\d{9}$|^[1-9]\d{9}$")) return BadRequest("Account num must be 10 digits");
            return Ok(_transactionService.MakeWithdrawal(AccNum, Amount, TPin));
        }


        [HttpPost]
        [Route("make_transfer")]
        public IActionResult MakeFundsTransfer(string FromAcc, string ToAcc, decimal Amount, string TPin)
        {
            if (!Regex.IsMatch(FromAcc, @"^[0][1-9]\d{9}$|^[1-9]\d{9}$") || !Regex.IsMatch(ToAcc, @"^[0][1-9]\d{9}$|^[1-9]\d{9}$")) return BadRequest("Account num must be 10 digits");

            return Ok(_transactionService.MakeFundsTransfer(FromAcc, ToAcc, Amount, TPin));
        }

        [HttpGet]
        [Route("view_transaction_history/{accNum}")]
        public IActionResult ViewTransactionsHistory(string accNum)
        {
            if(!Regex.IsMatch(accNum, @"^[0][1-9]\d{9}$|^[1-9]\d{9}$")) return BadRequest("Account num must be 10 digits");

            var transactions = _transactionService.GetTransactionHistory(accNum);
            if (transactions == null || transactions.Count == 0)
                return NotFound("No transactions done from the account: " + accNum);

            //var tDTO = _mapper.Map<List<TransactionDto>>(transactions);
            return Ok(transactions);

        }

    }
}
