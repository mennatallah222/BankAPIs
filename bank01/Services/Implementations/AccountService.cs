using bank01.DAL;
using bank01.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace bank01.Services.Implementations
{
    public class AccountService : IAccount
    {
        private BankDbContext _bankDbContext;
        public AccountService(BankDbContext bankDbContext)
        {
            _bankDbContext = bankDbContext;
        }
        public Account Aunthenticate(string AccNum, string Pin)
        {
            var acc = _bankDbContext.Accounts.Where(x => x.AccountNumberGenerated == AccNum).SingleOrDefault();
            if (acc == null) return null;

            if(!VerifyPinHash(Pin, acc.PinHash, acc.PinSalt)) return null;

            return acc;
        }

        private bool VerifyPinHash(string Pin, byte[] pinHash, byte[] pinSalt)
        {
            if (string.IsNullOrWhiteSpace(Pin)) throw new ArgumentNullException("Pin");
            using(var hmac=new System.Security.Cryptography.HMACSHA512(pinSalt))
            {
                var computedPinHash=hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(Pin));
                for(int i=0; i<computedPinHash.Length; i++)
                {
                    if (computedPinHash[i] != pinHash[i]) return false;
                }
            }
            return true;
        }

        public Account Create(Account account, string Pin, string ConfirmPin)
        {
            if (_bankDbContext.Accounts.Any(x => x.Email == account.Email))throw new ApplicationException("This email is already used");

            if (!Pin.Equals(ConfirmPin)) throw new ArgumentException("Pins don't match!", "Pin");

            byte[] pinHash, pinSalt;
            CreatePinHash(Pin, out pinHash, out pinSalt);//we're encrypting pin, first

            account.PinHash = pinHash;
            account.PinSalt = pinSalt;

            //now add the new account
            _bankDbContext.Accounts.Add(account);
            _bankDbContext.SaveChanges();

            return account;
        }

        private static void CreatePinHash(string pin, out byte[] pinHash, out byte[] pinSalt)
        {
            using (var hmac = new System.Security.Cryptography.HMACSHA512())
            {
                pinSalt = hmac.Key;
                pinHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(pin));
            }

        }

        public void Delete(int id)
        {
            var acc = _bankDbContext.Accounts.Find(id);
            if (acc != null)
            {
                _bankDbContext.Accounts.Remove(acc);
                _bankDbContext.SaveChanges();
            }
        }

        public Account GetAccountNum(string AccountNum)
        {
            var acc = _bankDbContext.Accounts.Where(x => x.AccountNumberGenerated == AccountNum).FirstOrDefault();
            if (acc == null) return null;
            return acc;
        }

        public IEnumerable<Account> GetAllAccounts()
        {
            return _bankDbContext.Accounts.ToList();
        }

        public Account GetById(int id)
        {
            var acc = _bankDbContext.Accounts.Where(x => x.Id == id).FirstOrDefault();
            if (acc == null) return null;
            return acc;
        }

        public void Update(Account account, string Pin = null)
        {
            // Find the account by ID
            var accTBU = _bankDbContext.Accounts.Where(x => x.Id == account.Id).SingleOrDefault();

            if (accTBU == null)
            {
                throw new ApplicationException("No account found with ID: " + account.Id);
            }

            // Update email (check for duplicates if needed)
            if (!string.IsNullOrWhiteSpace(account.Email))
            {
                if (_bankDbContext.Accounts.Any(x => x.Id != account.Id && x.Email == account.Email)) // Exclude current account ID
                {
                    throw new ApplicationException("This email " + account.Email + " already exists");
                }
                accTBU.Email = account.Email;
            }

            // Update phone number (check for duplicates if needed)
            if (!string.IsNullOrWhiteSpace(account.PhoneNumber))
            {
                if (_bankDbContext.Accounts.Any(x => x.Id != account.Id && x.PhoneNumber == account.PhoneNumber)) // Exclude current account ID
                {
                    throw new ApplicationException("This phone number " + account.PhoneNumber + " already exists");
                }
                accTBU.PhoneNumber = account.PhoneNumber;
            }

            // Update PIN (if provided)
            if (!string.IsNullOrWhiteSpace(Pin))
            {
                byte[] pinHash, pinSalt;
                CreatePinHash(Pin, out pinHash, out pinSalt);
                accTBU.PinHash = pinHash;
                accTBU.PinSalt = pinSalt;
            }

            accTBU.DateLastUpdated = DateTime.Now;

            // Update the account in the database
            _bankDbContext.Accounts.Update(accTBU);
            _bankDbContext.SaveChanges();
        }

        public decimal GetBalance(string AccountNum)
        {
            var acc = _bankDbContext.Accounts.Where(x => x.AccountNumberGenerated == AccountNum).FirstOrDefault();
            if (acc == null) return 0;
            return acc.CurrentBalance;
        }
    }
}
