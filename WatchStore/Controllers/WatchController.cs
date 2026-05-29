namespace WatchStore.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class WatchController(
    IWatchService watchService,
    IMapper mapper,
    IValidator<CreateWatchRequest> createValidator,
    IValidator<UpdateWatchRequest> updateValidator) : ControllerBase
{
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateWatchRequest request)
    {
        var validation = await createValidator.ValidateAsync(request);
        if (!validation.IsValid) return BadRequest(validation.Errors);

        var watchEntity = mapper.Map<Watch>(request);
        var createdWatch = await watchService.AddWatchAsync(watchEntity);

        var response = mapper.Map<WatchResponse>(createdWatch);
        return CreatedAtAction(nameof(GetById), new { id = createdWatch.Id }, response);
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var watches = await watchService.GetAllWatchesAsync();
        return Ok(mapper.Map<IEnumerable<WatchResponse>>(watches));
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var watch = await watchService.GetWatchByIdAsync(id);
        if (watch is null) return NotFound();

        return Ok(mapper.Map<WatchResponse>(watch));
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateWatchRequest request)
    {
        var validation = await updateValidator.ValidateAsync(request);
        if (!validation.IsValid) return BadRequest(validation.Errors);

        await watchService.UpdateWatchAsync(id, request);

        return NoContent();
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        await watchService.DeleteWatchAsync(id);
        return NoContent();
    }
}