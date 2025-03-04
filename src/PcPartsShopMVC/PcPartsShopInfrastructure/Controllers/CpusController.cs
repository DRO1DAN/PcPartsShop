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
    public class CpusController : Controller
    {
        private readonly PcPartsShopContext _context;

        public CpusController(PcPartsShopContext context)
        {
            _context = context;
        }

        // GET: Cpus
        public async Task<IActionResult> Index()
        {
            var pcPartsShopContext = _context.Cpus.Include(c => c.Product);
            return View(await pcPartsShopContext.ToListAsync());
        }

        // GET: Cpus/Details/5
        public async Task<IActionResult> Details(long? id)
        {
            if (id == null) return NotFound();

            var cpu = await _context.Cpus
                .Include(c => c.Product)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (cpu == null) return NotFound();

            return View(cpu);
        }

        // GET: Cpus/Create
        public IActionResult Create(long productId)
        {
            var product = _context.Products.FirstOrDefault(p => p.Id == productId);

            if (product == null) return NotFound("Product not found");

            var cpu = new Cpu
            {
                ProductId = product.Id,
                Name = product.Name
            };

            return View(cpu);
        }

        // POST: Cpus/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Name,Series,Generation,Cores,Threads,BaseClock,BoostClock,Cache,SupportedMemoryType,Socket,Tdp,ProductId,Id")] Cpu cpu)
        {
            cpu.Product = _context.Products
                .Include(p => p.Brand)
                .FirstOrDefault(p => p.Id == cpu.ProductId);

            ModelState.Clear();
            TryValidateModel(cpu);

            if (ModelState.IsValid)
            {
                _context.Add(cpu);
                await _context.SaveChangesAsync();
                return RedirectToAction("Details", "Products", new { id = cpu.ProductId });
            }

            return View(cpu);
        }

        // GET: Cpus/Edit/5
        public async Task<IActionResult> Edit(long? id)
        {
            if (id == null) return NotFound();

            var cpu = await _context.Cpus
                .Include(c => c.Product)
                .FirstOrDefaultAsync(c => c.Id == id);
            if (cpu == null) return NotFound();

            return View(cpu);
        }

        // POST: Cpus/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(long id, [Bind("Name,Series,Generation,Cores,Threads,BaseClock,BoostClock,Cache,SupportedMemoryType,Socket,Tdp,ProductId,Id")] Cpu cpu)
        {
            if (id != cpu.Id) return NotFound();

            cpu.Product = await _context.Products
                    .Include(p => p.Brand)
                    .FirstOrDefaultAsync(p => p.Id == cpu.ProductId);

            if (cpu.Product == null)
                return NotFound("Associated Product not found.");

            ModelState.Clear();
            TryValidateModel(cpu);

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(cpu);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!_context.Cpus.Any(e => e.Id == cpu.Id))
                        return NotFound();
                    else
                        throw;
                }
                return RedirectToAction("Details", "Products", new { id = cpu.ProductId });
            }

            return View(cpu);
        }

        // GET: Cpus/Delete/5
        public async Task<IActionResult> Delete(long? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var cpu = await _context.Cpus
                .Include(c => c.Product)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (cpu == null)
            {
                return NotFound();
            }

            return View(cpu);
        }

        // POST: Cpus/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(long id)
        {
            var cpu = await _context.Cpus.FindAsync(id);
            if (cpu != null)
            {
                _context.Cpus.Remove(cpu);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool CpuExists(long id)
        {
            return _context.Cpus.Any(e => e.Id == id);
        }
    }
}
