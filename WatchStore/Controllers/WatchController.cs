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
    public IActionResult Create([FromBody] CreateWatchRequest request)
    {
        var validation = createValidator.Validate(request);
        if (!validation.IsValid) return BadRequest(validation.Errors);

        var watchEntity = mapper.Map<Watch>(request);
        var createdWatch = watchService.AddWatch(watchEntity);

        var response = mapper.Map<WatchResponse>(createdWatch);
        return CreatedAtAction(nameof(GetById), new { id = createdWatch.Id }, response);
    }

    [HttpGet]
    public IActionResult GetAll()
    {
        var watches = watchService.GetAllWatches();
        return Ok(mapper.Map<IEnumerable<WatchResponse>>(watches));
    }

    [HttpGet("{id:guid}")]
    public IActionResult GetById(Guid id)
    {
        var watch = watchService.GetWatchById(id);
        if (watch is null) return NotFound();

        return Ok(mapper.Map<WatchResponse>(watch));
    }

    [HttpPut("{id:guid}")]
    public IActionResult Update(Guid id, [FromBody] UpdateWatchRequest request)
    {
        var validation = updateValidator.Validate(request);
        if (!validation.IsValid) return BadRequest(validation.Errors);

        watchService.UpdateWatch(id, request);

        return NoContent();
    }

    [HttpDelete("{id:guid}")]
    public IActionResult Delete(Guid id)
    {
        watchService.DeleteWatch(id);
        return NoContent();
    }
}