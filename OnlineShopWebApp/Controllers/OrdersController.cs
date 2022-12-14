using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using OnlineShopWebApp.DataModels;
using OnlineShopWebApp.Repositories;

namespace OnlineShopWebApp.Controllers
{
    public class OrdersController : Controller
    {
        public readonly IOrderRepository _orderRepository;
        public readonly IClientRepository _clientRepository;
        public readonly IOrderedProductRepository _OrderedProductRepository;
        public readonly IStorageRepository _storageRepository;

        public OrdersController(IOrderRepository orderRepository,
            IClientRepository clientRepository,
            IOrderedProductRepository orderedProductRepository,
            IStorageRepository storageRepository)
        {
            _orderRepository = orderRepository;
            _clientRepository = clientRepository;
            _OrderedProductRepository = orderedProductRepository;
            _storageRepository = storageRepository;
        }


        // GET: Orders
        public async Task<IActionResult> Index()
        {
            var orders = await _orderRepository.GetAll();
            return View(orders);
        }


        // GET: Orders/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return BadRequest();
            }

            var order = await _orderRepository.Get(id);
            if (order == null)
            {
                return NotFound();
            }

            return View(order);
        }


        // GET: Orders/Create
        public async Task<IActionResult> Create()
        {
            ViewData["ClientId"] = new SelectList(await _clientRepository.GetAll(), "Id", "Name");
            return View();
        }


        // POST: Orders/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,ClientId,Created")] Order order)
        {
            order.Client = await _clientRepository.Get(order.ClientId);

            if (ModelState.IsValid)
            {
                await _orderRepository.Add(order);
                return RedirectToAction(nameof(Index));
            }

            ViewData["ClientId"] = new SelectList(await _clientRepository.GetAll(), "Id", "Name", order.ClientId);
            return View(order);
        }


        // GET: Orders/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var order = await _orderRepository.Get(id);

            if (order == null)
            {
                return NotFound();
            }

            ViewData["ClientId"] = new SelectList(await _clientRepository.GetAll(), "Id", "Name");

            return View(order);
        }


        // POST: Orders/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,ClientId,Created")] Order order)
        {
            if (id != order.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    await _orderRepository.Update(order);
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!await OrderExists(order.Id))
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
            ViewData["ClientId"] = new SelectList(await _clientRepository.GetAll(), "Id", "Id", order.ClientId);
            return View(order);
        }


        // GET: Orders/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return BadRequest();
            }

            var order = await _orderRepository.Get(id);

            if (order == null)
            {
                return NotFound();
            }

            return View(order);
        }


        // POST: Orders/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {

            if (await _orderRepository.GetAll() == null)
            {
                return Problem("Entity set 'ShopContext.Clients' is null.");
            }

            var order = await _orderRepository.Get(id);

            if (order != null)
            {
                var orderProductsWithQuantity = await _OrderedProductRepository.GetProductsWithQuantityFromOrder(id);

                foreach (var item in orderProductsWithQuantity)
                {
                    await _storageRepository.IncreaseQuantity(item.ProductId, item.Quantity);
                }

                await _orderRepository.Delete(id);

            }

            return RedirectToAction(nameof(Index));
           
        }


        private async Task<bool> OrderExists(int id)
        {
            return await _orderRepository.IfExists(id);
        }
    }
}
