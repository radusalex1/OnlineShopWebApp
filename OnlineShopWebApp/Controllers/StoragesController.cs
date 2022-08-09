﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using OnlineShopWebApp.DataModels;
using OnlineShopWebApp.Repositories;

namespace OnlineShopWebApp.Controllers
{
    public class StoragesController : Controller
    {
        private readonly IStorageRepository _storageRepository;
        private readonly IProductRepository _productRepository;


        public StoragesController(IStorageRepository repo, IProductRepository productRepository)
        {
            _storageRepository = repo;
            _productRepository = productRepository;
        }


        // GET: Storages
        public async Task<IActionResult> Index()
        {
            return View(await _storageRepository.GetAll());
        }


        // GET: Storages/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var client = await _storageRepository.Get(id);

            if (client == null)
            {
                return NotFound();
            }

            return View(client);
        }


        // GET: Storages/Create
        public IActionResult Create()
        {
            ViewData["ProductId"] = new SelectList(_productRepository.GetAll().Result, "Id", "Name");
            return View();
        }


        // POST: Storages/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,ProductId,Quantity")] Storage storage)
        {
            if (ModelState.IsValid)
            {
                await _storageRepository.Add(storage);
                return RedirectToAction(nameof(Index));
            }
            ViewData["ProductId"] = new SelectList(_productRepository.GetAll().Result, "Id", "Name", storage.Product);
            return View(storage);
        }


        // GET: Storages/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var storage = await _storageRepository.Get(id);
            if (storage == null)
            {
                return NotFound();
            }
            ViewData["ProductId"] = new SelectList(_productRepository.GetAll().Result, "Id", "Name", storage.Product);
            return View(storage);
        }


        // POST: Storages/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,ProductId,Quantity")] Storage storage)
        {
            if (id != storage.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    await _storageRepository.Update(storage);
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!StorageExists(storage.Id))
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
            ViewData["ProductId"] = new SelectList(_productRepository.GetAll().Result, "Id", "Name", storage.Product);
            return View(storage);
        }


        // GET: Storages/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var storage = await _storageRepository.Get(id);

            if (storage == null)
            {
                return NotFound();
            }

            return View(storage);
        }


        // POST: Storages/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (await _storageRepository.GetAll() == null)
            {
                return Problem("Entity set 'ShopContext.Storages' is null.");
            }

            var storage = await _storageRepository.GetAll();

            if (storage != null)
            {
                await _storageRepository.Delete(id);
            }

            return RedirectToAction(nameof(Index));
        }


        private bool StorageExists(int id)
        {
            return _storageRepository.IfExists(id);
        }
    }
}
