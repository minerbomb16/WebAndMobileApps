using Microsoft.AspNetCore.Mvc;
using OnlineStore.Web.Data;

[ApiController]
[Route("api/mobile/orders")]
public class MobileOrdersController : ControllerBase
{
    private readonly OnlineStoreContext _context;

    public MobileOrdersController(OnlineStoreContext context)
    {
        _context = context;
    }

    [HttpGet]
    public IActionResult GetOrders()
    {
        var orders = _context.Orders.ToList();
        return Ok(orders);
    }
}
