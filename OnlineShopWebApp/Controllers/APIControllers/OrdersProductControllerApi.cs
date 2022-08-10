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
            return Ok(await _ordersProductRepository.GetProductsFromOrder(orderId));
        }

        [HttpPost("AddProdutsToOrder")]
        public async Task<IActionResult> AddProductToOrder(int orderId,List<int> produtIds)
        {
            return Ok();
        }
    }
}
