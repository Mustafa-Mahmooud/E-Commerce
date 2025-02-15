using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using Talabat.APIs.DTOs;
using Talabat.Core.Entities;
using Talabat.Core.Repositories_Interfaces;
using Talabat.Core.Specification;

namespace Talabat.APIs.Controllers
{
    [Authorize]
    public class OrdersController : BaseApiController
    {
        private readonly IGenericRepository<Order> _orderRepository;

        public OrdersController(IGenericRepository<Order> orderRepository)
        {
            _orderRepository = orderRepository;
        }

        [HttpPost]
        public async Task<ActionResult<Order>> CreateOrder(OrderDTO orderDto)
        {
            var email = User.FindFirstValue(ClaimTypes.Email);

            var address = new Address
            {
                Fname = orderDto.ShippingAddress.FirstName,
                Lname = orderDto.ShippingAddress.LastName,
                Street = orderDto.ShippingAddress.Street,
                City = orderDto.ShippingAddress.City,
                Country = orderDto.ShippingAddress.State,
               
            };

            var orderItems = new List<OrderItem>();
            foreach (var item in orderDto.OrderItems)
            {
                var productItem = new ProductItemOrdered(item.ProductId, item.ProductName, item.PictureUrl);
                var orderItem = new OrderItem(productItem, item.Price, item.Quantity);
                orderItems.Add(orderItem);
            }

            var order = new Order(email, address, orderItems, orderItems.Sum(i => i.Price * i.Quantity));
            var createdOrder = await _orderRepository.AddAsync(order);

            return Ok(createdOrder);
        }
       
        
        [HttpGet]
        public async Task<ActionResult<IReadOnlyList<Order>>> GetOrdersForUser()
        {
            var email = User.FindFirstValue(ClaimTypes.Email);

            // Create a specification to filter orders by buyer email
            var spec = new OrderSpecificaition<Order>(o => o.BuyerEmail == email);
            spec.AddInclude(o => o.OrderItems);
            spec.AddInclude(o => o.ShippingAddress);

            var orders = await _orderRepository.GetAllWithSpec(spec );
            return Ok(orders);
        }

       
        [HttpGet("{id}")]
        public async Task<ActionResult<Order>> GetOrderById(int id)
        {
            var email = User.FindFirstValue(ClaimTypes.Email);

            // Create a specification to filter orders by ID and buyer email
            var spec = new OrderSpecificaition<Order>(o => o.Id == id && o.BuyerEmail == email);
            spec.AddInclude(o => o.OrderItems);
            spec.AddInclude(o => o.ShippingAddress);

            var order = await _orderRepository.GetWithSpec(spec);
            if (order == null) return NotFound();

            return Ok(order);
        }
    }
}