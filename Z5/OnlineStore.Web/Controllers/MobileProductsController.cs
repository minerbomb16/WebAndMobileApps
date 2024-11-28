using Microsoft.AspNetCore.Mvc;
using OnlineStore.Web.Data;
using Microsoft.EntityFrameworkCore;


[ApiController]
[Route("api/mobile/products")]
public class MobileProductsController : ControllerBase
{
    private readonly OnlineStoreContext _context;

    public MobileProductsController(OnlineStoreContext context)
    {
        _context = context;
    }

    [HttpGet]
    public IActionResult GetProducts()
    {
        var products = _context.Products
            .Include(p => p.Category) // Load the Category data
            .Select(p => new
            {
                p.ProductId,
                p.Name,
                p.Price,
                Category = new
                {
                    p.Category.CategoryId,
                    p.Category.Name
                }
            })
            .ToList();

        return Ok(products);
    }

}
