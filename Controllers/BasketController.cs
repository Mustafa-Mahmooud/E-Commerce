using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Talabat.Core.Entities;
using Talabat.Core.Repositories_Interfaces;
using Talabat.Repository.Repositories;

namespace Talabat.APIs.Controllers
{

    public class BasketController : BaseApiController
    {
        private readonly IBasketCustomer _basketCustomer;

        public BasketController(IBasketCustomer basketCustomer)
        {
            _basketCustomer = basketCustomer;
        }

        [HttpGet]

        public async Task<ActionResult<CustomerBasket>> GetBasket(string id)
        {
            var basket = await _basketCustomer.GetBasketAsync(id);

            if (basket == null) { return BadRequest(); }
            return Ok(basket ?? new CustomerBasket(id));

        }

        [HttpPost]

        public async Task<ActionResult<CustomerBasket>> CreateOrUpdateBasket(CustomerBasket customerBasket)
        {
            var basket = await _basketCustomer.UpdateBasketAsync(customerBasket);
            if (basket == null) { return BadRequest(); }
            return Ok(basket);
        }


        [HttpDelete]
        public async Task<ActionResult<CustomerBasket>> DeleteBrand(string id)
        {

         var flag =  await _basketCustomer.DeleteBasketAsync(id);
           if(flag) return NoContent();
           else return BadRequest();


        }

    }
}
