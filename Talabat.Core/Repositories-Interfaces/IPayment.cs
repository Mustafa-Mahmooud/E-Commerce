﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Talabat.Core.Entities;

namespace Talabat.Core.Repositories_Interfaces
{
    public interface IPayment
    {
        Task<CustomerBasket?> CreateOrUpdatePayment(string BasketId);
    }
}
