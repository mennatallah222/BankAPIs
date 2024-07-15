using AutoMapper;
using bank01.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace bank01.Profiles
{
    public class AutomapperProfile:Profile
    {
        public AutomapperProfile()
        {
            CreateMap<RegisterNewAccModel, Account>();
            CreateMap<UpdateAccModel, Account>();
            CreateMap<Account, GetAccModel>();
            CreateMap<TransactionDto, Transaction>();
            //CreateMap<Transaction, TransactionDto>();

        }

    }
}
