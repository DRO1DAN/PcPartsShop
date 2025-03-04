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
    public class RamsController : Controller
    {
        private readonly PcPartsShopContext _context;

        public RamsController(PcPartsShopContext context)
        {
            _context = context;
        }

        // GET: Rams
        public async Task<IActionResult> Index()
        {
            var pcPartsShopContext = _context.Rams.Include(r => r.Product);
            return View(await pcPartsShopContext.ToListAsync());
        }

        // GET: Rams/Details/5
        public async Task<IActionResult> Details(long? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var ram = await _context.Rams
                .Include(r => r.Product)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (ram == null)
            {
                return NotFound();
            }

            return View(ram);
        }

        // GET: Rams/Create
        public IActionResult Create(long productId)
        {
            var product = _context.Products.FirstOrDefault(p => p.Id == productId);
            if (product == null) return NotFound("Product not found.");

            var ram = new Ram
            {
                ProductId = product.Id,
                Name = product.Name
            };

            return View(ram);
        }
        // POST: Rams/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Name,Capacity,Speed,Type,ProductId,Id")] Ram ram)
        {
            ram.Product = _context.Products.Include(p => p.Brand).FirstOrDefault(p => p.Id == ram.ProductId);
            ModelState.Clear();
            TryValidateModel(ram);

            if (ModelState.IsValid)
            {
                _context.Add(ram);
                await _context.SaveChangesAsync();
                return RedirectToAction("Details", "Products", new { id = ram.ProductId });
            }

            return View(ram);
        }

        // GET: Rams/Edit/5
        public async Task<IActionResult> Edit(long? id)
        {
            if (id == null) return NotFound();

            var ram = await _context.Rams
                .Include(c => c.Product)
                .FirstOrDefaultAsync(c => c.Id == id);
            if (ram == null) return NotFound();

            return View(ram);
        }

        // POST: Rams/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(long id, [Bind("Name,Capacity,Speed,Type,ProductId,Id")] Ram ram)
        {
            if (id != ram.Id) return NotFound();

            ram.Product = await _context.Products
                    .Include(p => p.Brand)
                    .FirstOrDefaultAsync(p => p.Id == ram.ProductId);

            if (ram.Product == null)
                return NotFound("Associated Product not found.");

            ModelState.Clear();
            TryValidateModel(ram);

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(ram);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!_context.Cpus.Any(e => e.Id == ram.Id))
                        return NotFound();
                    else
                        throw;
                }
                return RedirectToAction("Details", "Products", new { id = ram.ProductId });
            }

            return View(ram);
        }

        // GET: Rams/Delete/5
        public async Task<IActionResult> Delete(long? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var ram = await _context.Rams
                .Include(r => r.Product)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (ram == null)
            {
                return NotFound();
            }

            return View(ram);
        }

        // POST: Rams/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(long id)
        {
            var ram = await _context.Rams.FindAsync(id);
            if (ram != null)
            {
                _context.Rams.Remove(ram);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool RamExists(long id)
        {
            return _context.Rams.Any(e => e.Id == id);
        }
    }
}