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
    public class MotherboardsController : Controller
    {
        private readonly PcPartsShopContext _context;

        public MotherboardsController(PcPartsShopContext context)
        {
            _context = context;
        }

        // GET: Motherboards
        public async Task<IActionResult> Index()
        {
            var pcPartsShopContext = _context.Motherboards.Include(m => m.Product);
            return View(await pcPartsShopContext.ToListAsync());
        }

        // GET: Motherboards/Details/5
        public async Task<IActionResult> Details(long? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var motherboard = await _context.Motherboards
                .Include(m => m.Product)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (motherboard == null)
            {
                return NotFound();
            }

            return View(motherboard);
        }

        // GET: Motherboards/Create
        public IActionResult Create(long productId)
        {
            var product = _context.Products.FirstOrDefault(p => p.Id == productId);
            if (product == null) return NotFound("Product not found.");

            var motherboard = new Motherboard
            {
                ProductId = product.Id,
                Name = product.Name
            };

            return View(motherboard);
        }

        // POST: Motherboards/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Name,Socket,Chipset,FormFactor,MemorySlotsCount,MemoryType,MaxMemory,PcieVersion,ProductId,Id")] Motherboard motherboard)
        {
            motherboard.Product = _context.Products.Include(p => p.Brand).FirstOrDefault(p => p.Id == motherboard.ProductId);
            ModelState.Clear();
            TryValidateModel(motherboard);

            if (ModelState.IsValid)
            {
                _context.Motherboards.Add(motherboard);
                await _context.SaveChangesAsync();
                return RedirectToAction("Details", "Products", new { id = motherboard.ProductId });
            }

            return View(motherboard);
        }

        // GET: Motherboards/Edit/5
        public async Task<IActionResult> Edit(long? id)
        {
            if (id == null) return NotFound();

            var motherboard = await _context.Motherboards
                .Include(c => c.Product)
                .FirstOrDefaultAsync(c => c.Id == id);
            if (motherboard == null) return NotFound();

            return View(motherboard);
        }

        // POST: Motherboards/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(long id, [Bind("Name,Socket,Chipset,FormFactor,MemorySlotsCount,MemoryType,MaxMemory,PcieVersion,ProductId,Id")] Motherboard motherboard)
        {
            if (id != motherboard.Id) return NotFound();

            motherboard.Product = await _context.Products
                    .Include(p => p.Brand)
                    .FirstOrDefaultAsync(p => p.Id == motherboard.ProductId);

            if (motherboard.Product == null)
                return NotFound("Associated Product not found.");

            ModelState.Clear();
            TryValidateModel(motherboard);

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(motherboard);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!_context.Cpus.Any(e => e.Id == motherboard.Id))
                        return NotFound();
                    else
                        throw;
                }
                return RedirectToAction("Details", "Products", new { id = motherboard.ProductId });
            }

            return View(motherboard);
        }

        // GET: Motherboards/Delete/5
        public async Task<IActionResult> Delete(long? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var motherboard = await _context.Motherboards
                .Include(m => m.Product)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (motherboard == null)
            {
                return NotFound();
            }

            return View(motherboard);
        }

        // POST: Motherboards/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(long id)
        {
            var motherboard = await _context.Motherboards.FindAsync(id);
            if (motherboard != null)
            {
                _context.Motherboards.Remove(motherboard);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool MotherboardExists(long id)
        {
            return _context.Motherboards.Any(e => e.Id == id);
        }
    }
}
