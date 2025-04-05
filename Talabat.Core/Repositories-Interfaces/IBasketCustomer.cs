using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Talabat.Core.Entities;

namespace Talabat.Core.Repositories_Interfaces
{
    public interface IBasketCustomer
    {
        Task<CustomerBasket> GetBasketAsync(string BasketId );
        Task<CustomerBasket> UpdateBasketAsync(CustomerBasket customerBasket);
        Task<bool> DeleteBasketAsync( string BasketId );

    }
}
