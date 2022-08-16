using Microsoft.AspNetCore.Mvc;
using OnlineShopWebApp.Repositories;

namespace OnlineShopWebApp.Controllers.APIControllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrdersProductControllerApi : ControllerBase
    {
        public readonly IOrdersProductRepository _ordersProductRepository;

        public OrdersProductControllerApi(IOrdersProductRepository ordersProductRepository)
        {
            _ordersProductRepository = ordersProductRepository;
        }

        [HttpGet("GetProductsByOrder")]
        public async Task<IActionResult> GetProductsByOrder(int orderId)
        {
            if(orderId < 1 || orderId==null)
            {
                return BadRequest("Invalid orderId!");
            }

            var result = await _ordersProductRepository.GetProductsFromOrder(orderId);

            if( result==null || result.Count==0)
            {
                return Ok($"No products for orderId:{orderId}!");
            }
            return Ok(result);
        }

    }
}
