using Microsoft.AspNetCore.Mvc;
using OnlineStore.Web.Data;

[ApiController]
[Route("api/mobile/categories")]
public class MobileCategoriesController : ControllerBase
{
    private readonly OnlineStoreContext _context;

    public MobileCategoriesController(OnlineStoreContext context)
    {
        _context = context;
    }

    [HttpGet]
    public IActionResult GetCategories()
    {
        var categories = _context.Categories.ToList();
        return Ok(categories);
    }
}
