using AutoMapper;
using bank01.Models;
using bank01.Services;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace bank01.Controllers
{
    public class AccountController:ControllerBase
    {
        public IAccount _accountService { get; set; }
        IMapper _mapper;

        public AccountController(IAccount account, IMapper mapper)
        {
            _accountService = account;
            _mapper = mapper;
        }

        [HttpPost]
        [Route("register_new_account")]
        public IActionResult RegisterNewAccount([FromBody] RegisterNewAccModel newAcc)
        {
            if (!ModelState.IsValid) return BadRequest(newAcc);
            var acc = _mapper.Map<Account>(newAcc);
            return Ok(_accountService.Create(acc, newAcc.Pin, newAcc.ConfirmPin));
        }

        [HttpGet]
        [Route("get_all_accounts")]
        public IActionResult GetAllAccounts()
        {
            var accounts = _accountService.GetAllAccounts();
            var cleanedAccounts = _mapper.Map <IList<GetAccModel >> (accounts);
            return Ok(cleanedAccounts);
        }

        [HttpPost]
        [Route("authenticate")]
        public IActionResult Authenticate([FromBody] AuthenticateModel model)
        {
            if (!ModelState.IsValid) return BadRequest(model);
            return Ok(_accountService.Aunthenticate(model.AccountNumber, model.Pin));
        }

        [HttpGet]
        [Route("Get_by_account_number")]
        public IActionResult GetAccountByNumber(string AccountNum)
        {
            if (!Regex.IsMatch(AccountNum, @"^[0][1-9]\d{9}$|^[1-9]\d{9}$")) return BadRequest("Account num must be 10 digits");
            var account = _accountService.GetAccountNum(AccountNum);
            var cleanedAcc = _mapper.Map<GetAccModel>(account);
            return Ok(cleanedAcc);
        }

        [HttpGet]
        [Route("Get_by_id")]
        public IActionResult GetAccountById(int id)
        {
            var account = _accountService.GetById(id);
            var cleanedAcc = _mapper.Map<GetAccModel>(account);
            return Ok(cleanedAcc);
        }

        [HttpPut]
        [Route("update_account")]
        public IActionResult UpdateAcc([FromBody] UpdateAccModel model)
        {
            if (!ModelState.IsValid) return BadRequest(model);
            var acc = _mapper.Map<Account>(model);

            _accountService.Update(acc, model.Pin);
            return Ok();
        }

        [HttpDelete]
        [Route("close_account/{id}")]
        public IActionResult CloseAccount(int generatedAccountNumber)
        {
            _accountService.Delete(generatedAccountNumber);
            return Ok("Account closed successfully");
        }


    }
}
