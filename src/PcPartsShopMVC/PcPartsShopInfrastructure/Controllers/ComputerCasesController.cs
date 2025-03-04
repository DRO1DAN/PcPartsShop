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
    public class ComputerCasesController : Controller
    {
        private readonly PcPartsShopContext _context;

        public ComputerCasesController(PcPartsShopContext context)
        {
            _context = context;
        }

        // GET: ComputerCases
        public async Task<IActionResult> Index()
        {
            var pcPartsShopContext = _context.Cases.Include(c => c.Product);
            return View(await pcPartsShopContext.ToListAsync());
        }

        // GET: ComputerCases/Details/5
        public async Task<IActionResult> Details(long? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var computerCase = await _context.Cases
                .Include(c => c.Product)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (computerCase == null)
            {
                return NotFound();
            }

            return View(computerCase);
        }

        // GET: ComputerCases/Create
        public IActionResult Create(long productId)
        {
            var product = _context.Products.FirstOrDefault(p => p.Id == productId);
            if (product == null) return NotFound("Product not found.");

            var computerCase = new ComputerCase
            {
                ProductId = product.Id,
                Name = product.Name
            };

            return View(computerCase);
        }

        // POST: ComputerCases/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Name,FormFactor,ProductId,Id")] ComputerCase computerCase)
        {
            computerCase.Product = _context.Products.Include(p => p.Brand).FirstOrDefault(p => p.Id == computerCase.ProductId);
            ModelState.Clear();
            TryValidateModel(computerCase);

            if (ModelState.IsValid)
            {
                _context.Add(computerCase);
                await _context.SaveChangesAsync();

                return RedirectToAction("Details", "Products", new { id = computerCase.ProductId });
            }

            return View(computerCase);
        }

        // GET: ComputerCases/Edit/5
        public async Task<IActionResult> Edit(long? id)
        {
            if (id == null) return NotFound();

            var computerCase = await _context.Cases
                .Include(c => c.Product)
                .FirstOrDefaultAsync(c => c.Id == id);
            if (computerCase == null) return NotFound();

            return View(computerCase);
        }

        // POST: ComputerCases/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(long id, [Bind("Name,FormFactor,ProductId,Id")] ComputerCase computerCase)
        {
            if (id != computerCase.Id) return NotFound();

            computerCase.Product = await _context.Products
                    .Include(p => p.Brand)
                    .FirstOrDefaultAsync(p => p.Id == computerCase.ProductId);

            if (computerCase.Product == null)
                return NotFound("Associated Product not found.");

            ModelState.Clear();
            TryValidateModel(computerCase);

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(computerCase);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!_context.Cpus.Any(e => e.Id == computerCase.Id))
                        return NotFound();
                    else
                        throw;
                }
                return RedirectToAction("Details", "Products", new { id = computerCase.ProductId });
            }

            return View(computerCase);
        }

        // GET: ComputerCases/Delete/5
        public async Task<IActionResult> Delete(long? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var computerCase = await _context.Cases
                .Include(c => c.Product)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (computerCase == null)
            {
                return NotFound();
            }

            return View(computerCase);
        }

        // POST: ComputerCases/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(long id)
        {
            var computerCase = await _context.Cases.FindAsync(id);
            if (computerCase != null)
            {
                _context.Cases.Remove(computerCase);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ComputerCaseExists(long id)
        {
            return _context.Cases.Any(e => e.Id == id);
        }
    }
}
