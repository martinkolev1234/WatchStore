namespace WatchStore.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class StoreController(
    IStoreService storeService) : ControllerBase
{
    [HttpPost("purchase")]
    public IActionResult Purchase([FromBody] PurchaseRequest request)
    {
        storeService.PurchaseWatch(request);

        return Ok(new { message = "Purchase successful!" });
    }

    [HttpPost("sell")]
    public IActionResult Sell([FromBody] SellRequest request)
    {
        storeService.SellWatchToStore(request);

        return Ok(new { message = "Watch sold to store successfully." });
    }
}