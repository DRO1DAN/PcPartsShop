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
    public class ProductsController : Controller
    {
        private readonly PcPartsShopContext _context;

        public ProductsController(PcPartsShopContext context)
        {
            _context = context;
        }

        // GET: Products
        public async Task<IActionResult> Index()
        {
            var pcPartsShopContext = _context.Products.Include(p => p.Brand);
            return View(await pcPartsShopContext.ToListAsync());
        }

        // GET: Products/Details/5
        public async Task<IActionResult> Details(long? id)
        {
            if (id == null) return NotFound();

            var product = await _context.Products.FirstOrDefaultAsync(p => p.Id == id);
            if (product == null) return NotFound();

            long? entityId = null;

            switch (product.Category)
            {
                case "CPU":
                    entityId = await _context.Cpus
                        .Where(c => c.ProductId == id)
                        .Select(c => (long?)c.Id) // Select the Cpu's Id
                        .FirstOrDefaultAsync();
                    if (entityId != null)
                        return RedirectToAction("Details", "Cpus", new { id = entityId });
                    break;

                case "GPU":
                    entityId = await _context.Gpus
                        .Where(g => g.ProductId == id)
                        .Select(g => (long?)g.Id)
                        .FirstOrDefaultAsync();
                    if (entityId != null)
                        return RedirectToAction("Details", "Gpus", new { id = entityId });
                    break;

                case "RAM":
                    entityId = await _context.Rams
                        .Where(r => r.ProductId == id)
                        .Select(r => (long?)r.Id)
                        .FirstOrDefaultAsync();
                    if (entityId != null)
                        return RedirectToAction("Details", "Rams", new { id = entityId });
                    break;

                case "Motherboard":
                    entityId = await _context.Motherboards
                        .Where(m => m.ProductId == id)
                        .Select(m => (long?)m.Id)
                        .FirstOrDefaultAsync();
                    if (entityId != null)
                        return RedirectToAction("Details", "Motherboards", new { id = entityId });
                    break;

                case "PSU":
                    entityId = await _context.Psus
                        .Where(p => p.ProductId == id)
                        .Select(p => (long?)p.Id)
                        .FirstOrDefaultAsync();
                    if (entityId != null)
                        return RedirectToAction("Details", "Psus", new { id = entityId });
                    break;

                case "Case":
                case "ComputerCase":
                    entityId = await _context.Cases
                        .Where(c => c.ProductId == id)
                        .Select(c => (long?)c.Id)
                        .FirstOrDefaultAsync();
                    if (entityId != null)
                        return RedirectToAction("Details", "ComputerCases", new { id = entityId });
                    break;

                case "Storage":
                    entityId = await _context.Storages
                        .Where(s => s.ProductId == id)
                        .Select(s => (long?)s.Id)
                        .FirstOrDefaultAsync();
                    if (entityId != null)
                        return RedirectToAction("Details", "Storages", new { id = entityId });
                    break;
            }

            return NotFound("No corresponding component found for this Product.");
        }

        // GET: Products/Create
        public IActionResult Create()
        {
            ViewData["BrandId"] = new SelectList(_context.Brands, "Id", "Name");

            var categories = new List<SelectListItem>
            {
                new SelectListItem { Value = "",            Text = "Select Category" },
                new SelectListItem { Value = "CPU",         Text = "CPU" },
                new SelectListItem { Value = "Motherboard", Text = "Motherboard" },
                new SelectListItem { Value = "GPU",         Text = "GPU" },
                new SelectListItem { Value = "RAM",         Text = "RAM" },
                new SelectListItem { Value = "Storage",     Text = "Storage" },
                new SelectListItem { Value = "PSU",         Text = "PSU" },
                new SelectListItem { Value = "Case",        Text = "Case" }
            };

            ViewBag.CategoryList = categories;

            return View();
        }

        // POST: Products/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Name,Price,Category,BrandId,Id")] Product product)
        {
            Brand brand = _context.Brands.FirstOrDefault(b => b.Id == product.BrandId);
            product.Brand = brand;

            ModelState.Clear();
            TryValidateModel(product);

            if (ModelState.IsValid)
            {
                _context.Add(product);
                await _context.SaveChangesAsync();

                switch (product.Category)
                {
                    case "CPU":
                        return RedirectToAction("Create", "Cpus", new { productId = product.Id });
                    case "Motherboard":
                        return RedirectToAction("Create", "Motherboards", new { productId = product.Id });

                    case "GPU":
                        return RedirectToAction("Create", "Gpus", new { productId = product.Id });

                    case "RAM":
                        return RedirectToAction("Create", "Rams", new { productId = product.Id });

                    case "Storage":
                        return RedirectToAction("Create", "Storages", new { productId = product.Id });

                    case "PSU":
                        return RedirectToAction("Create", "Psus", new { productId = product.Id });

                    case "Case":
                        return RedirectToAction("Create", "ComputerCases", new { productId = product.Id });

                    default:
                        // If no matching category, just go to Product list
                        return RedirectToAction(nameof(Index));
                }

                //return RedirectToAction(nameof(Index));
            }
            ViewData["BrandId"] = new SelectList(_context.Brands, "Id", "Name", product.BrandId);

            ViewBag.CategoryList = new List<SelectListItem>
            {
                new SelectListItem { Value = "",       Text = "-- Select Category --" },
                new SelectListItem { Value = "CPU",    Text = "CPU" },
                new SelectListItem { Value = "Motherboard",    Text = "Motherboard" },
                new SelectListItem { Value = "GPU",    Text = "GPU" },
                new SelectListItem { Value = "RAM",    Text = "RAM" },
                new SelectListItem { Value = "Storage",Text = "Storage" },
                new SelectListItem { Value = "PSU",    Text = "PSU" },
                new SelectListItem { Value = "Case",   Text = "Case" }
            };
            return View(product);
        }

        public async Task<IActionResult> CleanupOrphanedProduct(long productId)
        {
            var product = await _context.Products.FindAsync(productId);
            if (product != null)
            {
                _context.Products.Remove(product);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction("Index");
        }

        // GET: Products/Edit/5
        public async Task<IActionResult> Edit(long? id)
        {
            if (id == null) return NotFound();

            var product = await _context.Products.Include(p => p.Brand).FirstOrDefaultAsync(p => p.Id == id);
            if (product == null) return NotFound();

            ViewBag.BrandId = _context.Brands
                .Select(b => new { Value = b.Id, Text = b.Name })
                .ToList();

            return View(product);
        }

        // POST: Products/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(long id, [Bind("Name,Price,Category,BrandId,Id")] Product product)
        {
            if (id != product.Id) return NotFound();

            var existingProduct = await _context.Products.FindAsync(id);
            if (existingProduct == null) return NotFound();

            product.Brand = await _context.Brands.FirstOrDefaultAsync(b => b.Id == product.BrandId);

            ModelState.Clear();
            TryValidateModel(product);

            if (!ModelState.IsValid)
                return View(product);

            try
            {
                existingProduct.Name = product.Name;
                existingProduct.Price = product.Price;
                existingProduct.BrandId = product.BrandId;

                _context.Update(existingProduct);
                await _context.SaveChangesAsync();

                switch (existingProduct.Category)
                {
                    case "CPU":
                        var cpu = await _context.Cpus.FirstOrDefaultAsync(c => c.ProductId == product.Id);
                        if (cpu != null)
                        {
                            cpu.Name = product.Name;
                            _context.Update(cpu);
                            await _context.SaveChangesAsync();
                            return RedirectToAction("Edit", "Cpus", new { id = cpu.Id });
                        }
                        break;

                    case "GPU":
                        var gpu = await _context.Gpus.FirstOrDefaultAsync(g => g.ProductId == product.Id);
                        if (gpu != null)
                        {
                            gpu.Name = product.Name;
                            _context.Update(gpu);
                            await _context.SaveChangesAsync();
                            return RedirectToAction("Edit", "Gpus", new { id = gpu.Id });
                        }
                        break;

                    case "RAM":
                        var ram = await _context.Rams.FirstOrDefaultAsync(r => r.ProductId == product.Id);
                        if (ram != null)
                        {
                            ram.Name = product.Name;
                            _context.Update(ram);
                            await _context.SaveChangesAsync();
                            return RedirectToAction("Edit", "Rams", new { id = ram.Id });
                        }
                        break;

                    case "Motherboard":
                        var motherboard = await _context.Motherboards.FirstOrDefaultAsync(m => m.ProductId == product.Id);
                        if (motherboard != null)
                        {
                            motherboard.Name = product.Name;
                            _context.Update(motherboard);
                            await _context.SaveChangesAsync();
                            return RedirectToAction("Edit", "Motherboards", new { id = motherboard.Id });
                        }
                        break;

                    case "PSU":
                        var psu = await _context.Psus.FirstOrDefaultAsync(p => p.ProductId == product.Id);
                        if (psu != null)
                        {
                            psu.Name = product.Name;
                            _context.Update(psu);
                            await _context.SaveChangesAsync();
                            return RedirectToAction("Edit", "Psus", new { id = psu.Id });
                        }
                        break;

                    case "Case":
                    case "ComputerCase":
                        var computerCase = await _context.Cases.FirstOrDefaultAsync(c => c.ProductId == product.Id);
                        if (computerCase != null)
                        {
                            computerCase.Name = product.Name;
                            _context.Update(computerCase);
                            await _context.SaveChangesAsync();
                            return RedirectToAction("Edit", "ComputerCases", new { id = computerCase.Id });
                        }
                        break;

                    case "Storage":
                        var storage = await _context.Storages.FirstOrDefaultAsync(s => s.ProductId == product.Id);
                        if (storage != null)
                        {
                            storage.Name = product.Name;
                            _context.Update(storage);
                            await _context.SaveChangesAsync();
                            return RedirectToAction("Edit", "Storages", new { id = storage.Id });
                        }
                        break;
                }
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!_context.Products.Any(e => e.Id == product.Id))
                    return NotFound();
                else
                    throw;
            }

            return RedirectToAction("Details", "Products", new { id = product.Id });
        }

        // GET: Products/Delete/5
        public async Task<IActionResult> Delete(long? id)
        {
            if (id == null) return NotFound();

            var product = await _context.Products
                .Include(p => p.Brand)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (product == null) return NotFound();

            return View(product);
        }

        // POST: Products/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(long id)
        {
            var product = await _context.Products.FindAsync(id);
            if (product == null) return NotFound();

            try
            {
                switch (product.Category)
                {
                    case "CPU":
                        var cpu = await _context.Cpus.FirstOrDefaultAsync(c => c.ProductId == id);
                        if (cpu != null) _context.Cpus.Remove(cpu);
                        break;

                    case "GPU":
                        var gpu = await _context.Gpus.FirstOrDefaultAsync(g => g.ProductId == id);
                        if (gpu != null) _context.Gpus.Remove(gpu);
                        break;

                    case "RAM":
                        var ram = await _context.Rams.FirstOrDefaultAsync(r => r.ProductId == id);
                        if (ram != null) _context.Rams.Remove(ram);
                        break;

                    case "Motherboard":
                        var motherboard = await _context.Motherboards.FirstOrDefaultAsync(m => m.ProductId == id);
                        if (motherboard != null) _context.Motherboards.Remove(motherboard);
                        break;

                    case "PSU":
                        var psu = await _context.Psus.FirstOrDefaultAsync(p => p.ProductId == id);
                        if (psu != null) _context.Psus.Remove(psu);
                        break;

                    case "Case":
                    case "ComputerCase":
                        var computerCase = await _context.Cases.FirstOrDefaultAsync(c => c.ProductId == id);
                        if (computerCase != null) _context.Cases.Remove(computerCase);
                        break;

                    case "Storage":
                        var storage = await _context.Storages.FirstOrDefaultAsync(s => s.ProductId == id);
                        if (storage != null) _context.Storages.Remove(storage);
                        break;
                }

                _context.Products.Remove(product);
                await _context.SaveChangesAsync();

                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                return BadRequest($"Error deleting product: {ex.Message}");
            }
        }

        private bool ProductExists(long id)
        {
            return _context.Products.Any(e => e.Id == id);
        }
    }
}
