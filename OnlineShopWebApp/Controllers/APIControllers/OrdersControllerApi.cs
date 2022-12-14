using Microsoft.AspNetCore.Mvc;
using OnlineShopWebApp.Repositories;

namespace OnlineShopWebApp.Controllers.APIControllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrdersControllerApi:ControllerBase
    {
        public readonly IOrderRepository _orderRepository;

        public OrdersControllerApi(IOrderRepository orderRepository)
        {
            _orderRepository = orderRepository;
        }

        [HttpPut("CancelOrderById")]
        public async Task<IActionResult> CancelOrderById(int? orderId)
        {
            if (orderId < 1 || orderId==null)
            {
                return BadRequest("Invalid OrderId!");
            }

            var result = await _orderRepository.CancelOrderById(orderId);

            if (!result)
            {
                return NotFound("Order not found!");
            }

            return Ok($"The order {orderId} was canceled successfully!");
        }

        [HttpPut("UnCancelOrderById")]
        public async Task<IActionResult> UnCancelOrderById(int? orderId)
        {
            if (orderId < 1 || orderId==null)
            {
                return BadRequest("Invalid OrderId!");
            }

            var result = await _orderRepository.UnCancelOrderById(orderId);

            if (!result)
            {
                return NotFound("Order not found!");
            }

            return Ok($"The order {orderId} was canceled successfully!");
        }

        [HttpGet("GetClientOrders")]
        public async Task<IActionResult> GetClientOrders(int? clientId)
        {
            if (clientId < 1 || clientId==null)
            {
                return BadRequest("Invalid clientId");
            }

            var result = await _orderRepository.GetOrdersByClientId(clientId);

            if (result == null || result.Count == 0)
            {
                return Ok($"No order found for clientId:{clientId}!");
            }

            return Ok(result);
        }

        [HttpGet("GetClientNumbersOfOrders")]
        public async Task<IActionResult> GetClientNumberOfOrders(int? clientId)
        {
            if (clientId < 1 || clientId==null)
            {
                return BadRequest("Invalid clientId");
            }

            var result = await _orderRepository.GetOrdersByClientId(clientId);

            if (result == null || result.Count == 0)
            {
                return Ok($"No order found for clientId:{clientId}!");
            }

            return Ok(result.Count);
        }

        [HttpGet("CheckIfOrderIsCanceled")]
        public async Task<IActionResult> CheckIfOrderIsCanceled(int? orderId)
        {
            if (orderId < 1 || orderId==null)
            {
                return BadRequest("Invalid orderId");
            }

            var result = await _orderRepository.Get(orderId);

            if (result == null)
            {
                return Ok($"No order found with id:{orderId}!");
            }

            if(result.Canceled)
            {
                return Ok($"The order is canceled");
            }
            else
            {
                return Ok($"The order is not canceled");
            }
        }
    }
}
