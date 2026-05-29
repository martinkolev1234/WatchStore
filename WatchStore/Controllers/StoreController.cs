namespace WatchStore.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class StoreController(
    IStoreService storeService) : ControllerBase
{
    [HttpPost("purchase")]
    public async Task<IActionResult> Purchase([FromBody] PurchaseRequest request)
    {
        await storeService.PurchaseWatchAsync(request);

        return Ok(new { message = "Purchase successful!" });
    }

    [HttpPost("sell")]
    public async Task<IActionResult> Sell([FromBody] SellRequest request)
    {
        await storeService.SellWatchToStoreAsync(request);

        return Ok(new { message = "Watch sold to store successfully." });
    }
}