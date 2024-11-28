using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using OnlineStore.Domain.Models;
using OnlineStore.Web.Data;

namespace OnlineStore.Web.Controllers
{
    public class OrdersController : Controller
    {
        private readonly OnlineStoreContext _context;

        public OrdersController(OnlineStoreContext context)
        {
            _context = context;
        }

        // GET: Orders
        public async Task<IActionResult> Index()
        {
            var orders = await _context.Orders
                .Include(o => o.OrderProducts)
                .ThenInclude(op => op.Product)
                .ToListAsync();

            return View(orders);
        }


        // GET: Orders/Create
        public IActionResult Create()
        {
            ViewData["Products"] = new SelectList(_context.Products, "ProductId", "Name");
            return View();
        }

        // POST: Orders/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Order order, int[] productIds, int[] quantities)
        {
            if (productIds == null || quantities == null || productIds.Length != quantities.Length)
            {
                ModelState.AddModelError("", "Invalid products or quantities.");
            }

            // Check for duplicate products
            if (productIds.GroupBy(p => p).Any(g => g.Count() > 1))
            {
                ModelState.AddModelError("", "Duplicate products are not allowed.");
            }

            // Check for empty product selections
            if (productIds.Any(p => p == 0))
            {
                ModelState.AddModelError("", "All product selections must be valid.");
            }

            if (ModelState.IsValid)
            {
                _context.Orders.Add(order);
                await _context.SaveChangesAsync();

                // Add products to the order
                for (int i = 0; i < productIds.Length; i++)
                {
                    var orderProduct = new OrderProduct
                    {
                        OrderId = order.OrderId,
                        ProductId = productIds[i],
                        Quantity = quantities[i]
                    };
                    _context.OrderProducts.Add(orderProduct);
                }

                await _context.SaveChangesAsync();

                return RedirectToAction(nameof(Index));
            }

            ViewData["Products"] = new SelectList(_context.Products, "ProductId", "Name");
            return View(order);
        }


        // GET: Orders/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var order = await _context.Orders
                .Include(o => o.OrderProducts)
                    .ThenInclude(op => op.Product)
                .FirstOrDefaultAsync(m => m.OrderId == id);

            if (order == null)
            {
                return NotFound();
            }

            return View(order);
        }

        // GET: Orders/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var order = await _context.Orders
                .Include(o => o.OrderProducts)
                .FirstOrDefaultAsync(o => o.OrderId == id);

            if (order == null)
            {
                return NotFound();
            }

            // Prepare data for view
            ViewData["Products"] = _context.Products.Select(p => new SelectListItem
            {
                Value = p.ProductId.ToString(),
                Text = p.Name
            }).ToList();

            return View(order);
        }


        // POST: Orders/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Order order, int[] productIds, int[] quantities)
        {
            if (id != order.OrderId)
            {
                return NotFound();
            }

            if (productIds == null || quantities == null || productIds.Length != quantities.Length)
            {
                ModelState.AddModelError("", "Invalid products or quantities.");
            }

            // Check for duplicate products
            if (productIds.GroupBy(p => p).Any(g => g.Count() > 1))
            {
                ModelState.AddModelError("", "Duplicate products are not allowed.");
            }

            // Check for empty product selections
            if (productIds.Any(p => p == 0))
            {
                ModelState.AddModelError("", "All product selections must be valid.");
            }

            if (ModelState.IsValid)
            {
                try
                {
                    // Update the order
                    _context.Update(order);

                    // Remove existing OrderProducts
                    var existingOrderProducts = _context.OrderProducts.Where(op => op.OrderId == id);
                    _context.OrderProducts.RemoveRange(existingOrderProducts);

                    // Add updated OrderProducts
                    for (int i = 0; i < productIds.Length; i++)
                    {
                        var orderProduct = new OrderProduct
                        {
                            OrderId = id,
                            ProductId = productIds[i],
                            Quantity = quantities[i]
                        };
                        _context.OrderProducts.Add(orderProduct);
                    }

                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!OrderExists(order.OrderId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }

                return RedirectToAction(nameof(Index));
            }

            // Reload products if model state is invalid
            ViewData["Products"] = new SelectList(_context.Products, "ProductId", "Name");
            return View(order);
        }


        // GET: Orders/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var order = await _context.Orders
                .Include(o => o.OrderProducts)
                    .ThenInclude(op => op.Product)
                .FirstOrDefaultAsync(m => m.OrderId == id);

            if (order == null)
            {
                return NotFound();
            }

            return View(order);
        }

        // POST: Orders/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var order = await _context.Orders
                .Include(o => o.OrderProducts)
                .FirstOrDefaultAsync(o => o.OrderId == id);

            if (order == null)
            {
                return NotFound();
            }

            // Remove associated OrderProducts
            _context.OrderProducts.RemoveRange(order.OrderProducts);

            // Remove the order
            _context.Orders.Remove(order);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool OrderExists(int id)
        {
            return _context.Orders.Any(e => e.OrderId == id);
        }
    }
}
