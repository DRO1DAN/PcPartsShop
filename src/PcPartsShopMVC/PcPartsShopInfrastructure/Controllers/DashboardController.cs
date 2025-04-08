using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace PcPartsShopInfrastructure.Controllers
{
    public class DashboardController : Controller
    {
        private readonly PcPartsShopContext _context;

        public DashboardController(PcPartsShopContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> GetProductCountByCategory()
        {
            var categoryCounts = await _context.Products
                .GroupBy(p => p.Category)
                .Select(g => new { Category = g.Key ?? "N/A", Count = g.Count() })
                .OrderBy(x => x.Category)
                .ToListAsync();

            var chartData = new List<object> { new object[] { "Category", "Count" } };
            categoryCounts.ForEach(item => chartData.Add(new object[] { item.Category, item.Count }));

            return Json(chartData);
        }

        [HttpGet]
        public async Task<IActionResult> GetProductCountByBrand()
        {
            var brandCounts = await _context.Products
               .Include(p => p.Brand)
               .GroupBy(p => p.Brand == null ? "N/A" : p.Brand.Name)
               .Select(g => new { Brand = g.Key, Count = g.Count() })
               .OrderBy(x => x.Brand)
               .ToListAsync();

            var chartData = new List<object> { new object[] { "Brand", "Count" } };
            brandCounts.ForEach(item => chartData.Add(new object[] { item.Brand, item.Count }));

            return Json(chartData);
        }
    }
}
