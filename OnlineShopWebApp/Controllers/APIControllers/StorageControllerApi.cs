using Microsoft.AspNetCore.Mvc;
using OnlineShopWebApp.Repositories;

namespace OnlineShopWebApp.Controllers.APIControllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class StorageControllerApi : ControllerBase
    {
        public readonly IStorageRepository _storageRepository;

        public StorageControllerApi(IStorageRepository storageRepository)
        {
            _storageRepository = storageRepository;
        }

        [HttpGet]
        public async Task<IActionResult> CheckAvailability(int productId)
        {
            if (productId < 1)
            {
                return BadRequest("Invalid productId");
            }

            var quantity = await _storageRepository.GetQuantityByProductId(productId);

            if (quantity == 0)
            {
                return NotFound($"Product {productId} is not on the stock");
            }

            return Ok(quantity);
        }
    }
}
