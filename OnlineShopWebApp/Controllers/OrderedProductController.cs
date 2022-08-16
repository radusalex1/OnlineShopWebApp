using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using OnlineShopWebApp.DataModels;
using OnlineShopWebApp.Repositories;

namespace OnlineShopWebApp.Controllers
{
    public class OrderedProductController : Controller
    {
        private readonly IOrderedProductRepository _orderedProductRepository;
        private readonly IOrderRepository _orderRepository;
        private readonly IProductRepository _productRepository;
        private readonly IStorageRepository _storageRepository;

        public OrderedProductController(IOrderedProductRepository OrderedProductRepository, 
            IOrderRepository orderRepository,
            IProductRepository productRepository, 
            IStorageRepository storageRepository)
        {
            _orderedProductRepository = OrderedProductRepository;
            _orderRepository = orderRepository;
            _productRepository = productRepository;
            _storageRepository = storageRepository;
        }


        // GET: OrderedProducts
        public async Task<IActionResult> Index()
        {
            return View(await _orderedProductRepository.GetAll());
        }


        // GET: OrderedProducts/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return BadRequest();
            }

            var orderedProduct = await _orderedProductRepository.Get(id);

            if (orderedProduct == null)
            {
                return NotFound();
            }

            return View(orderedProduct);
        }


        // GET: OrderedProducts/Create
        public IActionResult Create()
        {
            ViewData["OrderId"] = new SelectList(_orderRepository.GetAll().Result, "Id", "Id");
            ViewData["ProductId"] = new SelectList(_productRepository.GetAll().Result, "Id", "Name");
            return View();
        }


        // POST: OrderedProducts/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,OrderId,ProductId,Quantity")] OrderedProduct orderedProduct)
        {
            if (ModelState.IsValid && orderedProduct.Quantity > 0 && await _orderedProductRepository.IfExists(0, orderedProduct.OrderId, orderedProduct.ProductId) == false)
            {
                await _orderedProductRepository.Add(orderedProduct);

                return RedirectToAction(nameof(Index));
            }

            ViewData["OrderId"] = new SelectList(_orderRepository.GetAll().Result, "Id", "Id", orderedProduct.Order);
            ViewData["ProductId"] = new SelectList(_productRepository.GetAll().Result, "Id", "Name", orderedProduct.Product);

            return View(orderedProduct);
        }


        // GET: OrderedProducts/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return BadRequest();
            }

            var orderedProduct = await _orderedProductRepository.Get(id);

            if (orderedProduct == null)
            {
                return NotFound();
            }

            ViewData["OrderId"] = new SelectList(_orderRepository.GetAll().Result, "Id", "Id");

            ViewData["ProductId"] = new SelectList(_productRepository.GetAll().Result, "Id", "Name");

            return View(orderedProduct);
        }


        // POST: OrderedProducts/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,OrderId,ProductId,Quantity")] OrderedProduct orderedProduct)
        {
            if (id != orderedProduct.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid && orderedProduct.Quantity > 0)
            {
                try
                {
                    if (await _orderedProductRepository.IfExists(id, orderedProduct.OrderId, orderedProduct.ProductId) == false)
                    {
                        var oldQuantity = await _orderedProductRepository.GetQuantityForProductFromOrder(orderedProduct.OrderId, orderedProduct.ProductId);

                        if (oldQuantity < orderedProduct.Quantity)
                        {
                            await _storageRepository.DecreaseQuantity(orderedProduct.ProductId, orderedProduct.Quantity - oldQuantity);
                        }
                        else
                        {
                            await _storageRepository.IncreaseQuantity(orderedProduct.ProductId, oldQuantity - orderedProduct.Quantity);
                        }
                        await _orderedProductRepository.Update(orderedProduct);
                    }
                    else
                    {
                        ViewData["OrderId"] = new SelectList(_orderRepository.GetAll().Result, "Id", "Id", orderedProduct.Order);

                        ViewData["ProductId"] = new SelectList(_productRepository.GetAll().Result, "Id", "Name", orderedProduct.Product);

                        return View(orderedProduct);
                    }
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!await OrderedProductExists(orderedProduct.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["OrderId"] = new SelectList(_orderRepository.GetAll().Result, "Id", "Id", orderedProduct.Order);
            ViewData["ProductId"] = new SelectList(_productRepository.GetAll().Result, "Id", "Name", orderedProduct.Product);
            return View(orderedProduct);
        }


        // GET: OrderedProducts/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return BadRequest();
            }

            var orderedProduct = await _orderedProductRepository.Get(id);

            if (orderedProduct == null)
            {
                return NotFound();
            }

            return View(orderedProduct);
        }


        // POST: OrderedProducts/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (await _orderedProductRepository.GetAll() == null)
            {
                return Problem("Entity set 'ShopContext.OrderedProducts'  is null.");
            }

            var orderedProduct = await _orderedProductRepository.Get(id);

            if (orderedProduct != null)
            {
                await _orderedProductRepository.Delete(id);
            }

            return RedirectToAction(nameof(Index));
        }


        public async Task<bool> OrderedProductExists(int id)
        {
            return await _orderedProductRepository.IfExists(id);
        }
    }
}
