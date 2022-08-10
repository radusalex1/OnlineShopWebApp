using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using OnlineShopWebApp.DataModels;
using OnlineShopWebApp.Repositories;

namespace OnlineShopWebApp.Controllers
{
    public class OrdersProductController : Controller
    {
        private readonly IOrdersProductRepository _orderProductRepository;
        private readonly IOrderRepository _orderRepository;
        private readonly IProductRepository _productRepository;
        private readonly IStorageRepository _storageRepository;


        public OrdersProductController(IOrdersProductRepository ordersProductRepository, IOrderRepository orderRepository, IProductRepository productRepository, IStorageRepository storageRepository)
        {
            _orderProductRepository = ordersProductRepository;
            _orderRepository = orderRepository;
            _productRepository = productRepository;
            _storageRepository = storageRepository;
        }


        // GET: OrderedProducts
        public async Task<IActionResult> Index()
        {
            return View(await _orderProductRepository.GetAll());
        }


        // GET: OrderedProducts/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var orderedProduct = await _orderProductRepository.Get(id);

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
            if (ModelState.IsValid && orderedProduct.Quantity > 0 && await _orderProductRepository.IfExists(0, orderedProduct.OrderId, orderedProduct.ProductId) == false)
            {
                await _orderProductRepository.Add(orderedProduct);

                await _storageRepository.DecreaseQuantity(orderedProduct.ProductId, orderedProduct.Quantity);

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
                return NotFound();
            }

            var orderedProduct = await _orderProductRepository.Get(id);

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
                    if (await _orderProductRepository.IfExists(id, orderedProduct.OrderId, orderedProduct.ProductId) == false)
                    {
                        var oldQuantity = await _orderProductRepository.GetQuantityForProductFromOrder(orderedProduct.OrderId, orderedProduct.ProductId);

                        if (oldQuantity < orderedProduct.Quantity)
                        {
                            await _storageRepository.DecreaseQuantity(orderedProduct.ProductId, orderedProduct.Quantity - oldQuantity);
                        }
                        else
                        {
                           await _storageRepository.IncreaseQuantity(orderedProduct.ProductId, oldQuantity - orderedProduct.Quantity);
                        }

                        await _orderProductRepository.Update(orderedProduct);

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
                    if (!OrderedProductExists(orderedProduct.Id))
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
                return NotFound();
            }

            var orderedProduct = await _orderProductRepository.Get(id);

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
            if (await _orderProductRepository.GetAll() == null)
            {
                return Problem("Entity set 'ShopContext.OrderedProducts'  is null.");
            }

            var orderedProduct = await _orderProductRepository.Get(id);

            if (orderedProduct != null)
            {
                await _storageRepository.IncreaseQuantity(orderedProduct.ProductId, orderedProduct.Quantity);

                await _orderProductRepository.Delete(id);
            }

            return RedirectToAction(nameof(Index));
        }


        private bool OrderedProductExists(int id)
        {
            return _orderProductRepository.IfExists(id);
        }
    }
}
