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
    public class GpusController : Controller
    {
        private readonly PcPartsShopContext _context;

        public GpusController(PcPartsShopContext context)
        {
            _context = context;
        }

        // GET: Gpus
        public async Task<IActionResult> Index()
        {
            var pcPartsShopContext = _context.Gpus.Include(g => g.Product);
            return View(await pcPartsShopContext.ToListAsync());
        }

        // GET: Gpus/Details/5
        public async Task<IActionResult> Details(long? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var gpu = await _context.Gpus
                .Include(g => g.Product)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (gpu == null)
            {
                return NotFound();
            }

            return View(gpu);
        }

        // GET: Gpus/Create
        public IActionResult Create(long productId)
        {
            var product = _context.Products
                           .FirstOrDefault(p => p.Id == productId);

            if (product == null) return NotFound("Product not found.");

            var gpu = new Gpu
            {
                ProductId = product.Id,
                Name = product.Name
            };

            return View(gpu);
        }

        // POST: Gpus/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Name,Series,Generation,MemoryAmount,MemoryType,BaseClock,BoostClock,PcieVersion,PcieCount,Tdp,ProductId,Id")] Gpu gpu)
        {
            gpu.Product = _context.Products.Include(p => p.Brand).FirstOrDefault(p => p.Id == gpu.ProductId);
            ModelState.Clear();
            TryValidateModel(gpu);

            if (ModelState.IsValid)
            {
                _context.Add(gpu);
                await _context.SaveChangesAsync();
                return RedirectToAction("Details", "Products", new { id = gpu.ProductId });
            }

            return View(gpu);
        }

        // GET: Gpus/Edit/5
        public async Task<IActionResult> Edit(long? id)
        {
            if (id == null) return NotFound();

            var gpu = await _context.Gpus
                .Include(c => c.Product)
                .FirstOrDefaultAsync(c => c.Id == id);
            if (gpu == null) return NotFound();

            return View(gpu);
        }

        // POST: Gpus/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(long id, [Bind("Name,Series,Generation,MemoryAmount,MemoryType,BaseClock,BoostClock,PcieVersion,PcieCount,Tdp,ProductId,Id")] Gpu gpu)
        {
            if (id != gpu.Id) return NotFound();

            gpu.Product = await _context.Products
                    .Include(p => p.Brand)
                    .FirstOrDefaultAsync(p => p.Id == gpu.ProductId);

            if (gpu.Product == null)
                return NotFound("Associated Product not found.");

            ModelState.Clear();
            TryValidateModel(gpu);

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(gpu);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!_context.Gpus.Any(e => e.Id == gpu.Id))
                        return NotFound();
                    else
                        throw;
                }
                return RedirectToAction("Details", "Products", new { id = gpu.ProductId });
            }

            return View(gpu);
        }

        // GET: Gpus/Delete/5
        public async Task<IActionResult> Delete(long? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var gpu = await _context.Gpus
                .Include(g => g.Product)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (gpu == null)
            {
                return NotFound();
            }

            return View(gpu);
        }

        // POST: Gpus/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(long id)
        {
            var gpu = await _context.Gpus.FindAsync(id);
            if (gpu != null)
            {
                _context.Gpus.Remove(gpu);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool GpuExists(long id)
        {
            return _context.Gpus.Any(e => e.Id == id);
        }
    }
}
