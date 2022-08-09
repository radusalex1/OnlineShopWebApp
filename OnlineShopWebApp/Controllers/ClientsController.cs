﻿using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using OnlineShopWebApp.DataModels;
using OnlineShopWebApp.Repositories;

namespace OnlineShopWebApp.Controllers
{
    public class ClientsController : Controller
    {
        private readonly IClientRepository _clientRepository;
        private readonly IGenderRepository _genderRepository;

        public ClientsController(IClientRepository repo, IGenderRepository genderRepository)
        {
            _clientRepository = repo;
            _genderRepository = genderRepository;
        }

        // GET: Clients
        public async Task<IActionResult> Index()
        {
            return View(await _clientRepository.GetAll());
        }

        // GET: Clients/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var client = await _clientRepository.Get(id);

            if (client == null)
            {
                return NotFound();
            }

            return View(client);
        }

        // GET: Clients/Create
        public IActionResult Create()
        {
            ViewData["GenderId"] = new SelectList(_genderRepository.GetAll().Result, "Id", "Id");
            return View();
        }

        // POST: Clients/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Name,Street,City,Country,PhoneNumber,GenderId")] Client client)
        {
            client.Gender = await _genderRepository.Get(client.GenderId);
            if (ModelState.IsValid)
            {
                await _clientRepository.Add(client);
                return RedirectToAction(nameof(Index));
            }
            ViewData["GenderId"] = new SelectList(_genderRepository.GetAll().Result, "Id", "Id", client.Gender);
            return View(client);
        }

        // GET: Clients/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var client = await _clientRepository.Get(id);

            if (client == null)
            {
                return NotFound();
            }
            ViewData["GenderType"] = new SelectList(_genderRepository.GetAll().Result, "Id", "Id", client.Gender);
            return View(client);
        }

        // POST: Clients/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,Street,City,Country,PhoneNumber,GenderId")] Client client)
        {
            if (id != client.Id)
            {
                return NotFound();
            }
            
            if (ModelState.IsValid)
            {
                try
                {
                    await _clientRepository.Update(client);
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ClientExists(client.Id))
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
            ViewData["GenderId"] = new SelectList(_genderRepository.GetAll().Result, "Id", "Id", client.Gender);
            return View(client);
        }


        // GET: Clients/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var client = await _clientRepository.Get(id);

            if (client == null)
            {
                return NotFound();
            }

            return View(client);
        }


        // POST: Clients/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            //    if (_clientRepository.GetAll() == null)
            //    {
            //        return Problem("Entity set 'ShopContext.Clients'  is null.");
            //    }

            var client = await _clientRepository.Get(id);
            if (client != null)
            {
                await _clientRepository.Delete(client.Id);
            }
            return RedirectToAction(nameof(Index));
        }


        private bool ClientExists(int id)
        {
            return _clientRepository.IfExists(id);
        }
    }
}
