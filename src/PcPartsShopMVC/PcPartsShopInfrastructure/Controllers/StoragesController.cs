using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using PcPartsShopDomain.Model;
using PcPartsShopInfrastructure;

namespace PcPartsShopInfrastructure.Controllers
{
    public class StoragesController : Controller
    {
        private readonly PcPartsShopContext _context;

        public StoragesController(PcPartsShopContext context)
        {
            _context = context;
        }

        // GET: Storages
        public async Task<IActionResult> Index()
        {
            var pcPartsShopContext = _context.Storages.Include(s => s.Product);
            return View(await pcPartsShopContext.ToListAsync());
        }

        // GET: Storages/Details/5
        public async Task<IActionResult> Details(long? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var storage = await _context.Storages
                .Include(s => s.Product)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (storage == null)
            {
                return NotFound();
            }

            return View(storage);
        }

        // GET: Storages/Create
        public IActionResult Create(long productId)
        {
            var product = _context.Products.FirstOrDefault(p => p.Id == productId);
            if (product == null)
            {
                return NotFound("Product not found.");
            }

            var storage = new Storage
            {
                ProductId = product.Id,
                Name = product.Name
            };

            return View(storage);
        }

        // POST: Storages/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Name,Type,Capacity,Interface,MemoryType,ProductId,Id")] Storage storage)
        {
            storage.Product = _context.Products.Include(p => p.Brand).FirstOrDefault(p => p.Id == storage.ProductId);
            ModelState.Clear();
            TryValidateModel(storage);
            if (ModelState.IsValid)
            {
                _context.Add(storage);
                await _context.SaveChangesAsync();
                return RedirectToAction("Details", "Products", new { id = storage.ProductId });
            }

            return View(storage);
        }

        // GET: Storages/Edit/5
        public async Task<IActionResult> Edit(long? id)
        {
            if (id == null) return NotFound();

            var storage = await _context.Storages
                .Include(c => c.Product)
                .FirstOrDefaultAsync(c => c.Id == id);
            if (storage == null) return NotFound();

            return View(storage);
        }

        // POST: Storages/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(long id, [Bind("Name,Type,Capacity,Interface,MemoryType,ProductId,Id")] Storage storage)
        {
            if (id != storage.Id) return NotFound();

            storage.Product = await _context.Products
                    .Include(p => p.Brand)
                    .FirstOrDefaultAsync(p => p.Id == storage.ProductId);

            if (storage.Product == null)
                return NotFound("Associated Product not found.");

            ModelState.Clear();
            TryValidateModel(storage);

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(storage);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!_context.Storages.Any(e => e.Id == storage.Id))
                        return NotFound();
                    else
                        throw;
                }
                return RedirectToAction("Details", "Products", new { id = storage.ProductId });
            }

            return View(storage);
        }

        // GET: Storages/Delete/5
        public async Task<IActionResult> Delete(long? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var storage = await _context.Storages
                .Include(s => s.Product)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (storage == null)
            {
                return NotFound();
            }

            return View(storage);
        }

        // POST: Storages/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(long id)
        {
            var storage = await _context.Storages.FindAsync(id);
            if (storage != null)
            {
                _context.Storages.Remove(storage);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool StorageExists(long id)
        {
            return _context.Storages.Any(e => e.Id == id);
        }
    }
}
