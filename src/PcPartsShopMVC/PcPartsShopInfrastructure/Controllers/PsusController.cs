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
    public class PsusController : Controller
    {
        private readonly PcPartsShopContext _context;

        public PsusController(PcPartsShopContext context)
        {
            _context = context;
        }

        // GET: Psus
        public async Task<IActionResult> Index()
        {
            var pcPartsShopContext = _context.Psus.Include(p => p.Product);
            return View(await pcPartsShopContext.ToListAsync());
        }

        // GET: Psus/Details/5
        public async Task<IActionResult> Details(long? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var psu = await _context.Psus
                .Include(p => p.Product)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (psu == null)
            {
                return NotFound();
            }

            return View(psu);
        }

        // GET: Psus/Create
        public IActionResult Create(long productId)
        {
            var product = _context.Products.FirstOrDefault(p => p.Id == productId);
            if (product == null) return NotFound("Product not found.");

            var psu = new Psu
            {
                ProductId = product.Id,
                Name = product.Name
            };

            return View(psu);
        }

        // POST: Psus/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Name,Wattage,Modular,Efficiency,ProductId,Id")] Psu psu)
        {
            psu.Product = _context.Products.Include(p => p.Brand).FirstOrDefault(p => p.Id == psu.ProductId);
            ModelState.Clear();
            TryValidateModel(psu);

            if (ModelState.IsValid)
            {
                _context.Add(psu);
                await _context.SaveChangesAsync();
                return RedirectToAction("Details", "Products", new { id = psu.ProductId });
            }

            return View(psu);
        }

        // GET: Psus/Edit/5
        public async Task<IActionResult> Edit(long? id)
        {
            if (id == null) return NotFound();

            var psu = await _context.Psus
                .Include(c => c.Product)
                .FirstOrDefaultAsync(c => c.Id == id);
            if (psu == null) return NotFound();

            return View(psu);
        }

        // POST: Psus/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(long id, [Bind("Name,Wattage,Modular,Efficiency,ProductId,Id")] Psu psu)
        {
            if (id != psu.Id) return NotFound();

            psu.Product = await _context.Products
                    .Include(p => p.Brand)
                    .FirstOrDefaultAsync(p => p.Id == psu.ProductId);

            if (psu.Product == null)
                return NotFound("Associated Product not found.");

            ModelState.Clear();
            TryValidateModel(psu);

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(psu);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!_context.Psus.Any(e => e.Id == psu.Id))
                        return NotFound();
                    else
                        throw;
                }
                return RedirectToAction("Details", "Products", new { id = psu.ProductId });
            }

            return View(psu);
        }

        // GET: Psus/Delete/5
        public async Task<IActionResult> Delete(long? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var psu = await _context.Psus
                .Include(p => p.Product)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (psu == null)
            {
                return NotFound();
            }

            return View(psu);
        }

        // POST: Psus/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(long id)
        {
            var psu = await _context.Psus.FindAsync(id);
            if (psu != null)
            {
                _context.Psus.Remove(psu);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool PsuExists(long id)
        {
            return _context.Psus.Any(e => e.Id == id);
        }
    }
}
