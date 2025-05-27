using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using CourseWorkServer.Data;
using CourseWorkServer.Models;
using System.Collections.ObjectModel;
using System.Xml.Linq;

namespace CourseWorkServer.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductsController : ControllerBase
    {
        private readonly ApplicationContext _context;

        public ProductsController(ApplicationContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<ObservableCollection<Product>>> GetProducts() // admin.cs
        {
            ObservableCollection<Product> Products = new ObservableCollection<Product>();
            if (_context.Products == null) return Ok(null);
            foreach (Product prod in _context.Products)
            {
                Products.Add(prod);
            }
            return Ok(Products);
        }

        [HttpGet("{name}")]
        public async Task<ActionResult<Product>> GetProduct(string name) // cart.cs
        {
            var product = await _context.Products
        .AsNoTracking()
        .FirstOrDefaultAsync(p => p.Name == name);
            if (product == null) return NotFound(null);
            return Ok(product);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteProduct(int id) // admin.cs
        {
            var prod = await _context.Products.FindAsync(id);
            if (prod == null) return NotFound("Товар не найден");
            _context.Products.Remove(prod);
            await _context.SaveChangesAsync();
            return NoContent();
        }

        [HttpPut("{edit}")]
        public async Task<ActionResult<Product>> EditProduct([FromBody] Product? product) // admin.cs
        {
            var prod = await _context.Products.FindAsync(product.id);
            if (prod == null) return NotFound("Такого товара не существует");
            prod.id = product.id;
            prod.Name = product.Name;
            prod.Description = product.Description;
            prod.Category = product.Category;
            prod.Cost = product.Cost;
            prod.Image = product.Image;
            await _context.SaveChangesAsync();
            return Ok("Изменения успешны");
        }

        [HttpPost]
        public async Task<ActionResult<Product>> PostProduct([FromBody] Product? prod) // admin.cs
        {
            _context.Products.Add(prod);
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(GetProduct), new { id = prod.id }, prod);
        }

        [HttpGet("getlastprod")]
        public async Task<ActionResult<Product>> GetLastProduct() // admin.cs
        {
            if (_context.Products == null) return Ok(null);
            return Ok(_context.Products.OrderByDescending(u => u.id).FirstOrDefault());
        }

        [HttpGet("{name}/getcost")]
        [Produces("application/json")]
        public async Task<ActionResult<string>> GetCost(string name) // cashier.cs
        {
            var prod = await _context.Products
        .AsNoTracking()
        .FirstOrDefaultAsync(p => p.Name == name);
            if (prod == null) return NotFound(null);
            return Ok(prod.Cost);
        }

        [HttpGet("{name}/getimage")]
        [Produces("application/json")]
        public async Task<ActionResult<string>> GetImage(string name) // cashier.cs
        {
            var prod = await _context.Products
        .AsNoTracking()
        .FirstOrDefaultAsync(p => p.Name == name);
            if (prod == null) return NotFound("Товар не найден");
            return Ok(prod.Image);
        }

        [HttpGet("get/{category}")]
        public async Task<ActionResult<IEnumerable<Product>>> GetCategoryMenu(string category) // menu.cs
        {
            var products = _context.Products
                                 .Where(p => p.Category.Contains(category))
                                 .AsNoTracking()
                                 .ToList();
            return Ok(products);
        }
    }
}
